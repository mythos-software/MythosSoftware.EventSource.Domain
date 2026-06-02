namespace MythosSoftware.EventSource.Domain.Events;

public sealed record Created(string Id, object o) : IDomainEvent;