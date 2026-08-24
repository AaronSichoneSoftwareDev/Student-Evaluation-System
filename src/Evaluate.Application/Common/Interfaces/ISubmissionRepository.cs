using Evaluate.Domain.Entities.Dashboard;

namespace Evaluate.Application.Common.Interfaces;

public interface ISubmissionRepository
{
    Task<IReadOnlyList<Submission>> GetAllAsync(CancellationToken cancellationToken = default);
}
