using MediatR;

namespace Evaluate.Application.Topics.Queries.GetTopicsList;

public record GetTopicsListQuery(int? CourseId = null) : IRequest<List<TopicDto>>;
