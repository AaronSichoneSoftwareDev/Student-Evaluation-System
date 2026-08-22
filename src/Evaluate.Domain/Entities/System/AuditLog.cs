using Evaluate.Domain.Common;

namespace Evaluate.Domain.Entities.System;

/// <summary>System-generated record of a create/update/delete, written automatically by
/// the persistence layer's save-changes interceptor — never created directly by application code.</summary>
public class AuditLog : BaseEntity
{
    public string? UserId { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string TableName { get; private set; } = string.Empty;
    public string? RecordId { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }

    private AuditLog()
    {
    }

    public static AuditLog Create(string? userId, string action, string tableName, string? recordId, string? oldValues, string? newValues) => new()
    {
        UserId = userId,
        Timestamp = DateTime.UtcNow,
        Action = action,
        TableName = tableName,
        RecordId = recordId,
        OldValues = oldValues,
        NewValues = newValues,
    };
}
