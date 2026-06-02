namespace MythosSoftware.EventSource.Domain.Events;

public sealed record AuditContext(string PerformedBy, string CorrelationId, string CausationId)
{
    public static AuditContext From(string performedBy, string? correlationId = null, string? causationId = null)
    {
        if (string.IsNullOrWhiteSpace(performedBy))
        {
            throw new ArgumentException("PerformedBy is required. Pass the user/system that caused the command.");
        }

        return new AuditContext(
            performedBy.Trim(),
            string.IsNullOrWhiteSpace(correlationId) ? Guid.NewGuid().ToString("N") : correlationId.Trim(),
            string.IsNullOrWhiteSpace(causationId) ? Guid.NewGuid().ToString("N") : causationId.Trim());
    }
}