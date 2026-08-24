using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Terms;
using Evaluate.Application.Terms.Queries.GetTermsList;
using Microsoft.EntityFrameworkCore;
using TermEntity = Evaluate.Domain.Entities.Academic.Term;

namespace Evaluate.Infrastructure.Repositories;

public class TermRepository(IApplicationDbContext context) : ITermRepository
{
    public Task<bool> ExistsAsync(int academicYearId, int termNumber, int? excludeId = null, CancellationToken cancellationToken = default) =>
        context.Terms.AnyAsync(t => t.AcademicYearId == academicYearId && t.TermNumber == termNumber && t.Id != excludeId, cancellationToken);

    public Task<bool> ExistsByNameAsync(int academicYearId, string termName, int? excludeId = null, CancellationToken cancellationToken = default) =>
        context.Terms.AnyAsync(t => t.AcademicYearId == academicYearId && t.TermName == termName && t.Id != excludeId, cancellationToken);

    public Task<TermEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Terms.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public Task<TermEntity?> GetCurrentAsync(int academicYearId, CancellationToken cancellationToken = default) =>
        context.Terms.FirstOrDefaultAsync(t => t.AcademicYearId == academicYearId && t.IsCurrent, cancellationToken);

    public void Add(TermEntity term) => context.Terms.Add(term);

    public Task<List<TermDto>> GetListAsync(int? academicYearId, CancellationToken cancellationToken = default)
    {
        var query = context.Terms.AsQueryable();
        if (academicYearId.HasValue)
        {
            query = query.Where(t => t.AcademicYearId == academicYearId);
        }

        return query
            .OrderBy(t => t.TermNumber)
            .Select(t => new TermDto(t.Id, t.AcademicYearId, t.TermName, t.TermNumber, t.StartDate, t.EndDate, t.IsActive, t.IsCurrent))
            .ToListAsync(cancellationToken);
    }
}
