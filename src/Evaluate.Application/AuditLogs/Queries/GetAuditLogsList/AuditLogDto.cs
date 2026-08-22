namespace Evaluate.Application.AuditLogs.Queries.GetAuditLogsList;

public record AuditLogDto(int Id, string? UserId, DateTime Timestamp, string Action, string TableName, string? RecordId);
