using Evaluate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Application.AuditLogs.Queries.GetAuditLogsList;

public class GetAuditLogsListQueryHandler(IApplicationDbContext context) : IRequestHandler<GetAuditLogsListQuery, List<AuditLogDto>>
{
    public async Task<List<AuditLogDto>> Handle(GetAuditLogsListQuery request, CancellationToken cancellationToken) =>
        await context.AuditLogs
            .OrderByDescending(a => a.Timestamp)
            .Take(request.Take)
            .Select(a => new AuditLogDto(a.Id, a.UserId, a.Timestamp, a.Action, a.TableName, a.RecordId))
            .ToListAsync(cancellationToken);
}
