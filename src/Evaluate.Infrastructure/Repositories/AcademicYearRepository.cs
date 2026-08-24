using Evaluate.Application.AcademicYears;
using Evaluate.Application.AcademicYears.Queries.GetAcademicYearsList;
using Evaluate.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using AcademicYearEntity = Evaluate.Domain.Entities.Academic.AcademicYear;

namespace Evaluate.Infrastructure.Repositories;

public class AcademicYearRepository(IApplicationDbContext context) : IAcademicYearRepository
{
    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        context.AcademicYears.AnyAsync(y => y.Id == id, cancellationToken);

    public Task<bool> ExistsByNameAsync(string yearName, int? excludeId = null, CancellationToken cancellationToken = default) =>
        context.AcademicYears.AnyAsync(y => y.YearName == yearName && y.Id != excludeId, cancellationToken);

    public Task<AcademicYearEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.AcademicYears.FirstOrDefaultAsync(y => y.Id == id, cancellationToken);

    public Task<AcademicYearEntity?> GetCurrentAsync(CancellationToken cancellationToken = default) =>
        context.AcademicYears.FirstOrDefaultAsync(y => y.IsCurrent, cancellationToken);

    public void Add(AcademicYearEntity academicYear) => context.AcademicYears.Add(academicYear);

    public Task<List<AcademicYearDto>> GetListAsync(CancellationToken cancellationToken = default) =>
        context.AcademicYears
            .OrderByDescending(y => y.StartDate)
            .Select(y => new AcademicYearDto(y.Id, y.YearName, y.StartDate, y.EndDate, y.IsCurrent, y.IsActive))
            .ToListAsync(cancellationToken);
}
