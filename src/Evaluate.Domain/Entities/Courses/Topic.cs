using Evaluate.Domain.Common;

namespace Evaluate.Domain.Entities.Courses;

public class Topic : BaseAuditableEntity
{
    public int CourseId { get; private set; }
    public Course? Course { get; private set; }
    public string TopicName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public int TopicOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    private Topic()
    {
    }

    private Topic(int courseId, string topicName, string? description, int topicOrder)
    {
        CourseId = courseId;
        TopicName = topicName;
        Description = description;
        TopicOrder = topicOrder;
    }

    public static Topic Create(int courseId, string topicName, int topicOrder, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(topicName))
        {
            throw new ArgumentException("Topic name is required.", nameof(topicName));
        }

        return new Topic(courseId, topicName.Trim(), description?.Trim(), topicOrder);
    }

    public void Deactivate() => IsActive = false;
}
