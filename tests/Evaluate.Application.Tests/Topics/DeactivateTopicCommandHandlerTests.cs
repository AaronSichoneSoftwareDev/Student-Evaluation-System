using Evaluate.Application.Topics.Commands.DeactivateTopic;
using Evaluate.Application.Tests.Common;
using Xunit;
using CourseEntity = Evaluate.Domain.Entities.Courses.Course;
using TopicEntity = Evaluate.Domain.Entities.Courses.Topic;

namespace Evaluate.Application.Tests.Topics;

public class DeactivateTopicCommandHandlerTests
{
    [Fact]
    public async Task Handle_MarksTopicInactive()
    {
        using var context = TestDbContext.Create();
        var course = CourseEntity.Create("MATH01", "Mathematics");
        context.Courses.Add(course);
        await context.SaveChangesAsync(CancellationToken.None);
        var topic = TopicEntity.Create(course.Id, "Algebra", 1);
        context.Topics.Add(topic);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new DeactivateTopicCommandHandler(RepositoryFactory.Topics(context), context);
        var result = await handler.Handle(new DeactivateTopicCommand(topic.Id), CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.False(context.Topics.Single(t => t.Id == topic.Id).IsActive);
    }

    [Fact]
    public async Task Handle_WithUnknownId_ThrowsNotFound()
    {
        using var context = TestDbContext.Create();
        var handler = new DeactivateTopicCommandHandler(RepositoryFactory.Topics(context), context);

        await Assert.ThrowsAsync<Evaluate.Application.Common.Exceptions.NotFoundException>(
            () => handler.Handle(new DeactivateTopicCommand(999), CancellationToken.None));
    }
}
