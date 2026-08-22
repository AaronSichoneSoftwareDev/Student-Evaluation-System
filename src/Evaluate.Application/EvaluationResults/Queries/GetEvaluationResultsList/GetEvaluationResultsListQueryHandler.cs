using Evaluate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Application.EvaluationResults.Queries.GetEvaluationResultsList;

public class GetEvaluationResultsListQueryHandler(IApplicationDbContext context) : IRequestHandler<GetEvaluationResultsListQuery, List<EvaluationResultDto>>
{
    public async Task<List<EvaluationResultDto>> Handle(GetEvaluationResultsListQuery request, CancellationToken cancellationToken)
    {
        var results = await context.EvaluationResults
            .Include(r => r.Topic)
            .Where(r => r.EvaluationId == request.EvaluationId)
            .ToListAsync(cancellationToken);

        return results
            .Select(r => new EvaluationResultDto(r.Id, r.EvaluationId, r.TopicId, r.Topic!.TopicName, r.Score, r.Comment))
            .ToList();
    }
}
