using Evaluate.Application.Classes;
using Evaluate.Application.Classes.Queries.GetClassesList;
using Evaluate.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using SchoolClassEntity = Evaluate.Domain.Entities.Academic.SchoolClass;

namespace Evaluate.Infrastructure.Repositories;

public class ClassRepository(IApplicationDbContext context) : IClassRepository
{
    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        context.Classes.AnyAsync(c => c.Id == id, cancellationToken);

    public Task<bool> ExistsByNameAsync(string className, CancellationToken cancellationToken = default) =>
        context.Classes.AnyAsync(c => c.ClassName == className, cancellationToken);

    public Task<SchoolClassEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Classes.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public void Add(SchoolClassEntity schoolClass) => context.Classes.Add(schoolClass);

    public Task<List<ClassDto>> GetListAsync(CancellationToken cancellationToken = default) =>
        context.Classes
            .OrderBy(c => c.ClassName)
            .Select(c => new ClassDto(c.Id, c.ClassName, c.GradeLevel, c.Description, c.IsActive))
            .ToListAsync(cancellationToken);
}
