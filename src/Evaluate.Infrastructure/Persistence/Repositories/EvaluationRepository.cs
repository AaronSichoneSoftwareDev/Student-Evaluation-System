using Evaluate.Application.Common.Interfaces;
using Evaluate.Domain.Entities.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Infrastructure.Persistence.Repositories;

public class EvaluationRepository(EvaluateDbContext context) : IEvaluationRepository
{
    public async Task<IReadOnlyList<Evaluation>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Evaluations.AsNoTracking().Include(e => e.Student).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Evaluation>> GetLatestAsync(int count, CancellationToken cancellationToken = default)
        => await context.Evaluations.AsNoTracking().Include(e => e.Student)
            .OrderByDescending(e => e.Date).ThenByDescending(e => e.Id)
            .Take(count).ToListAsync(cancellationToken);
}
