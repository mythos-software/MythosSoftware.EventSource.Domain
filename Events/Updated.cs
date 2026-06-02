namespace MythosSoftware.EventSource.Domain.Events;

public sealed record Updated(string Id, object o) : IDomainEvent;