using Evaluate.Application.Common.Interfaces;
using Evaluate.Domain.Entities.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Infrastructure.Persistence.Repositories;

public class InstructorRepository(EvaluateDbContext context) : IInstructorRepository
{
    public async Task<Instructor?> GetFeaturedAsync(CancellationToken cancellationToken = default)
        => await context.Instructors.AsNoTracking()
            .Where(i => i.Quote != string.Empty)
            .FirstOrDefaultAsync(cancellationToken);
}
