using MythosSoftware.EventSource.Domain.Events;

namespace MythosSoftware.EventSource.Domain.Entities;

public abstract class EntityBase
{
    #region Fields

    private readonly List<EventEnvelope> _pending = [];

    #endregion

    #region Properties

    public string Id { get; set; }

    public bool IsDeleted { get; set; }

    public int Version { get; private set; }

    public bool Exsists { get; protected set; }

    public IReadOnlyCollection<EventEnvelope> PendingEvents => _pending.AsReadOnly();

    #endregion

    #region Public Methods

    public void MarkSaved() => _pending.Clear();

    public static T Rehydrate<T>(string id, IEnumerable<IDomainEvent> history) where T : EntityBase
    {
        var entity = (T)Activator.CreateInstance(typeof(T), nonPublic: true)!;
        entity.Id = id;

        foreach (var e in history)
        {
            entity.Apply(e, isReplay: true);
        }

        return entity;
    }
    
    public void Create(string id, object o, AuditContext audit)
    {
        Raise(new Created(id, o), audit);
    }
    
    public void Update(string id, object o, AuditContext audit)
    {
        Raise(new Updated(id, o), audit);
    }
    
    public void Delete(string id, AuditContext audit)
    {
        Raise(new Deleted(id), audit);
    }

    #endregion

    #region Protected Methods

    protected void Raise(IDomainEvent e, AuditContext audit)
    {
        Apply(e, isReplay: false);

        _pending.Add(new EventEnvelope(e, new EventMetadata(audit.PerformedBy, audit.CorrelationId, audit.CausationId, DateTimeOffset.UtcNow)));
    }

    protected abstract void Apply(IDomainEvent ev);
    
    #endregion

    #region Private Methods

    private void Apply(IDomainEvent ev, bool isReplay)
    {
        var handled = false;

        switch (ev)
        {
            case Created:
                Id = ev.Id;
                Exsists = true;
                IsDeleted = false;
                handled = true;
                MapFromEvent(ev, [nameof(Id), nameof(Exsists), nameof(IsDeleted)]);
                break;
            case Updated:
                handled = true;
                MapFromEvent(ev, [nameof(Id)]);
                break;
            case Deleted:
                IsDeleted = true;
                handled = true;
                break;
        }

        if (!handled)
        {
            Apply(ev);
        }

        Version++;
    }

    void MapFromEvent(object o, List<string> ignoreProperties)
    {
        var properties = o.GetType().GetProperties().Where(p => !ignoreProperties.Contains(p.Name));

        foreach (var property in properties)
        {
            GetType().GetProperty(property.Name)?.SetValue(this, property.GetValue(o));
        }
    }

    #endregion
}