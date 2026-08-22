using Evaluate.Application.AcademicYears.Queries.GetAcademicYearsList;
using AcademicYearEntity = Evaluate.Domain.Entities.Academic.AcademicYear;

namespace Evaluate.Application.AcademicYears;

public interface IAcademicYearRepository
{
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(string yearName, CancellationToken cancellationToken = default);

    Task<AcademicYearEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<AcademicYearEntity?> GetCurrentAsync(CancellationToken cancellationToken = default);

    void Add(AcademicYearEntity academicYear);

    Task<List<AcademicYearDto>> GetListAsync(CancellationToken cancellationToken = default);
}
