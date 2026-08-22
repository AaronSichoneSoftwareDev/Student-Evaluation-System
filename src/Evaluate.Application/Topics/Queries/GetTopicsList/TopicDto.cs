namespace Evaluate.Application.Topics.Queries.GetTopicsList;

public record TopicDto(int Id, int CourseId, string TopicName, string? Description, int TopicOrder, bool IsActive);
