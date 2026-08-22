using MediatR;

namespace Evaluate.Application.AuditLogs.Queries.GetAuditLogsList;

// Not permission-gated yet, like every other list query in this codebase — there's no
// login UI to authenticate against yet, so an enforced read permission would just make
// this page permanently empty. Revisit once a real auth flow exists.
public record GetAuditLogsListQuery(int Take = 100) : IRequest<List<AuditLogDto>>;
