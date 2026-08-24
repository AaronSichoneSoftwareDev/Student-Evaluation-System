using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Topics;
using Evaluate.Application.Topics.Queries.GetTopicsList;
using Microsoft.EntityFrameworkCore;
using TopicEntity = Evaluate.Domain.Entities.Courses.Topic;

namespace Evaluate.Infrastructure.Repositories;

public class TopicRepository(IApplicationDbContext context) : ITopicRepository
{
    public void Add(TopicEntity topic) => context.Topics.Add(topic);

    public Task<TopicEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Topics.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public Task<int> CountValidForCourseAsync(List<int> topicIds, int courseId, CancellationToken cancellationToken = default) =>
        context.Topics.CountAsync(t => topicIds.Contains(t.Id) && t.CourseId == courseId && t.IsActive, cancellationToken);

    public Task<List<TopicDto>> GetListAsync(int? courseId, CancellationToken cancellationToken = default)
    {
        var query = context.Topics.AsQueryable();
        if (courseId.HasValue)
        {
            query = query.Where(t => t.CourseId == courseId);
        }

        return query
            .OrderBy(t => t.TopicOrder)
            .Select(t => new TopicDto(t.Id, t.CourseId, t.TopicName, t.Description, t.TopicOrder, t.IsActive))
            .ToListAsync(cancellationToken);
    }
}
