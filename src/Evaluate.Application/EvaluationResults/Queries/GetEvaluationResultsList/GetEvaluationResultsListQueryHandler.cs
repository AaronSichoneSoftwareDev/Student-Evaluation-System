using Evaluate.Application.Evaluations;
using MediatR;

namespace Evaluate.Application.EvaluationResults.Queries.GetEvaluationResultsList;

public class GetEvaluationResultsListQueryHandler(IEvaluationRepository evaluations) : IRequestHandler<GetEvaluationResultsListQuery, List<EvaluationResultDto>>
{
    public Task<List<EvaluationResultDto>> Handle(GetEvaluationResultsListQuery request, CancellationToken cancellationToken) =>
        evaluations.GetResultsByEvaluationIdAsync(request.EvaluationId, cancellationToken);
}
