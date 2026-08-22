using MediatR;

namespace Evaluate.Application.EvaluationCriteria.Queries.GetEvaluationCriteriaList;

public class GetEvaluationCriteriaListQueryHandler(IEvaluationCriteriaRepository criteria) : IRequestHandler<GetEvaluationCriteriaListQuery, List<EvaluationCriteriaDto>>
{
    public Task<List<EvaluationCriteriaDto>> Handle(GetEvaluationCriteriaListQuery request, CancellationToken cancellationToken) =>
        criteria.GetListAsync(request.CourseId, cancellationToken);
}
