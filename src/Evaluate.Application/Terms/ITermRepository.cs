using Evaluate.Application.Terms.Queries.GetTermsList;
using TermEntity = Evaluate.Domain.Entities.Academic.Term;

namespace Evaluate.Application.Terms;

public interface ITermRepository
{
    /// <summary>Pass <paramref name="excludeId"/> when checking on an update, so the entity
    /// being edited doesn't collide with itself.</summary>
    Task<bool> ExistsAsync(int academicYearId, int termNumber, int? excludeId = null, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(int academicYearId, string termName, int? excludeId = null, CancellationToken cancellationToken = default);

    Task<TermEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<TermEntity?> GetCurrentAsync(int academicYearId, CancellationToken cancellationToken = default);

    void Add(TermEntity term);

    Task<List<TermDto>> GetListAsync(int? academicYearId, CancellationToken cancellationToken = default);
}
