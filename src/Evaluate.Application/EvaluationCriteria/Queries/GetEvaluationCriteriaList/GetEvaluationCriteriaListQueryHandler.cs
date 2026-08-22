using Evaluate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Application.EvaluationCriteria.Queries.GetEvaluationCriteriaList;

public class GetEvaluationCriteriaListQueryHandler(IApplicationDbContext context) : IRequestHandler<GetEvaluationCriteriaListQuery, List<EvaluationCriteriaDto>>
{
    public async Task<List<EvaluationCriteriaDto>> Handle(GetEvaluationCriteriaListQuery request, CancellationToken cancellationToken)
    {
        var query = context.EvaluationCriteria.AsQueryable();

        if (request.CourseId.HasValue)
        {
            query = query.Where(c => c.CourseId == request.CourseId);
        }

        return await query
            .OrderBy(c => c.CriteriaName)
            .Select(c => new EvaluationCriteriaDto(c.Id, c.CourseId, c.CriteriaName, c.Description, c.MaxScore, c.Weight, c.IsActive))
            .ToListAsync(cancellationToken);
    }
}
