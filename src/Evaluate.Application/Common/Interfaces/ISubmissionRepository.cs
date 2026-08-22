using Evaluate.Domain.Entities.Dashboard;

namespace Evaluate.Application.Common.Interfaces;

public interface ISubmissionRepository
{
    Task<IReadOnlyList<Submission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Submission>> GetLatestAsync(int count, CancellationToken cancellationToken = default);
}
