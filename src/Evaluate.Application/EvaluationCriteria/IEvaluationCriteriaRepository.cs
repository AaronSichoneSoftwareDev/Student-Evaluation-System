using Evaluate.Application.EvaluationCriteria.Queries.GetEvaluationCriteriaList;
using EvaluationCriteriaEntity = Evaluate.Domain.Entities.Evaluations.EvaluationCriteria;

namespace Evaluate.Application.EvaluationCriteria;

public interface IEvaluationCriteriaRepository
{
    Task<decimal> GetActiveWeightSumAsync(int courseId, CancellationToken cancellationToken = default);

    void Add(EvaluationCriteriaEntity criteria);

    Task<List<EvaluationCriteriaDto>> GetListAsync(int? courseId, CancellationToken cancellationToken = default);
}
