using Evaluate.Application.Common.Interfaces;
using Evaluate.Domain.Entities.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Infrastructure.Persistence.Repositories;

public class InboxRepository(EvaluateDbContext context) : IInboxRepository
{
    public async Task<IReadOnlyList<InboxMessage>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.InboxMessages.AsNoTracking().OrderByDescending(m => m.Time).ToListAsync(cancellationToken);
}
