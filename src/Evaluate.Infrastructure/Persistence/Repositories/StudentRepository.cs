using Evaluate.Application.Common.Interfaces;
using Evaluate.Domain.Entities.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Infrastructure.Persistence.Repositories;

public class StudentRepository(EvaluateDbContext context) : IStudentRepository
{
    public async Task<IReadOnlyList<Student>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Students.AsNoTracking().ToListAsync(cancellationToken);
}
