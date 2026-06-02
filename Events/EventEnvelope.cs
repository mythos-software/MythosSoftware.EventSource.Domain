namespace MythosSoftware.EventSource.Domain.Events;

public sealed record EventEnvelope(IDomainEvent Data, EventMetadata Metadata);