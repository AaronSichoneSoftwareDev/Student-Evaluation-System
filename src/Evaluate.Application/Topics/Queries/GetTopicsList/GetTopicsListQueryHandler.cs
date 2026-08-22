using Evaluate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Application.Topics.Queries.GetTopicsList;

public class GetTopicsListQueryHandler(IApplicationDbContext context) : IRequestHandler<GetTopicsListQuery, List<TopicDto>>
{
    public async Task<List<TopicDto>> Handle(GetTopicsListQuery request, CancellationToken cancellationToken)
    {
        var query = context.Topics.AsQueryable();

        if (request.CourseId.HasValue)
        {
            query = query.Where(t => t.CourseId == request.CourseId);
        }

        return await query
            .OrderBy(t => t.TopicOrder)
            .Select(t => new TopicDto(t.Id, t.CourseId, t.TopicName, t.Description, t.TopicOrder, t.IsActive))
            .ToListAsync(cancellationToken);
    }
}
