using Evaluate.Application.AuditLogs;
using Evaluate.Application.AuditLogs.Queries.GetAuditLogsList;
using Evaluate.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Infrastructure.Repositories;

public class AuditLogRepository(IApplicationDbContext context) : IAuditLogRepository
{
    public Task<List<AuditLogDto>> GetListAsync(int take, CancellationToken cancellationToken = default) =>
        context.AuditLogs
            .OrderByDescending(a => a.Timestamp)
            .Take(take)
            .Select(a => new AuditLogDto(a.Id, a.UserId, a.Timestamp, a.Action, a.TableName, a.RecordId))
            .ToListAsync(cancellationToken);
}
