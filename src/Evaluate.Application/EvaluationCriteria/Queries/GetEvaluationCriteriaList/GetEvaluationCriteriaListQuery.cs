using MediatR;

namespace Evaluate.Application.EvaluationCriteria.Queries.GetEvaluationCriteriaList;

public record GetEvaluationCriteriaListQuery(int? CourseId = null) : IRequest<List<EvaluationCriteriaDto>>;
