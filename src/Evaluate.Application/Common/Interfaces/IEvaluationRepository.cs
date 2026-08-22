using Evaluate.Domain.Entities.Dashboard;

namespace Evaluate.Application.Common.Interfaces;

public interface IEvaluationRepository
{
    Task<IReadOnlyList<Evaluation>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Evaluation>> GetLatestAsync(int count, CancellationToken cancellationToken = default);
}
