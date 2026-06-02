namespace MythosSoftware.EventSource.Domain.Events;

public sealed record Deleted(string Id) : IDomainEvent;