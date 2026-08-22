using Evaluate.Application.AuditLogs.Queries.GetAuditLogsList;

namespace Evaluate.Application.AuditLogs;

public interface IAuditLogRepository
{
    Task<List<AuditLogDto>> GetListAsync(int take, CancellationToken cancellationToken = default);
}
