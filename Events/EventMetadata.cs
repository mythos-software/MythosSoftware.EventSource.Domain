namespace MythosSoftware.EventSource.Domain.Events;

public sealed record EventMetadata(string PerformedBy, string CorrelationId, string CausationId, DateTimeOffset TimestampUtc);