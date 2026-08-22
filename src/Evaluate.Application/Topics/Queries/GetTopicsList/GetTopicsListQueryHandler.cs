using MediatR;

namespace Evaluate.Application.Topics.Queries.GetTopicsList;

public class GetTopicsListQueryHandler(ITopicRepository topics) : IRequestHandler<GetTopicsListQuery, List<TopicDto>>
{
    public Task<List<TopicDto>> Handle(GetTopicsListQuery request, CancellationToken cancellationToken) =>
        topics.GetListAsync(request.CourseId, cancellationToken);
}
