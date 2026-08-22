using MediatR;

namespace Evaluate.Application.EvaluationResults.Queries.GetEvaluationResultsList;

public record GetEvaluationResultsListQuery(int EvaluationId) : IRequest<List<EvaluationResultDto>>;
