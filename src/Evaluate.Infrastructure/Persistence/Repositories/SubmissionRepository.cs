using Evaluate.Application.Common.Interfaces;
using Evaluate.Domain.Entities.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Infrastructure.Persistence.Repositories;

public class SubmissionRepository(EvaluateDbContext context) : ISubmissionRepository
{
    public async Task<IReadOnlyList<Submission>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Submissions.AsNoTracking().Include(s => s.Student).ToListAsync(cancellationToken);
}
