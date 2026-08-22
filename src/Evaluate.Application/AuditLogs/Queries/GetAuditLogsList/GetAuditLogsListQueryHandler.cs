using MediatR;

namespace Evaluate.Application.AuditLogs.Queries.GetAuditLogsList;

public class GetAuditLogsListQueryHandler(IAuditLogRepository auditLogs) : IRequestHandler<GetAuditLogsListQuery, List<AuditLogDto>>
{
    public Task<List<AuditLogDto>> Handle(GetAuditLogsListQuery request, CancellationToken cancellationToken) =>
        auditLogs.GetListAsync(request.Take, cancellationToken);
}
