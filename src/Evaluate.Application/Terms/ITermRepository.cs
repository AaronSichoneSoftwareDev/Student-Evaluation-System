using Evaluate.Application.Terms.Queries.GetTermsList;
using TermEntity = Evaluate.Domain.Entities.Academic.Term;

namespace Evaluate.Application.Terms;

public interface ITermRepository
{
    Task<bool> ExistsAsync(int academicYearId, int termNumber, CancellationToken cancellationToken = default);

    Task<TermEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<TermEntity?> GetCurrentAsync(int academicYearId, CancellationToken cancellationToken = default);

    void Add(TermEntity term);

    Task<List<TermDto>> GetListAsync(int? academicYearId, CancellationToken cancellationToken = default);
}
