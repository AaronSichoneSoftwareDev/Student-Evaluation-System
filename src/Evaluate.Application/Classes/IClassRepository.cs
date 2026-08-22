using Evaluate.Application.Classes.Queries.GetClassesList;
using SchoolClassEntity = Evaluate.Domain.Entities.Academic.SchoolClass;

namespace Evaluate.Application.Classes;

public interface IClassRepository
{
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(string className, CancellationToken cancellationToken = default);

    Task<SchoolClassEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    void Add(SchoolClassEntity schoolClass);

    Task<List<ClassDto>> GetListAsync(CancellationToken cancellationToken = default);
}
