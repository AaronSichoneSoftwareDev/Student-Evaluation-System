using Evaluate.Application.Topics.Queries.GetTopicsList;
using TopicEntity = Evaluate.Domain.Entities.Courses.Topic;

namespace Evaluate.Application.Topics;

public interface ITopicRepository
{
    void Add(TopicEntity topic);

    Task<int> CountValidForCourseAsync(List<int> topicIds, int courseId, CancellationToken cancellationToken = default);

    Task<List<TopicDto>> GetListAsync(int? courseId, CancellationToken cancellationToken = default);
}
