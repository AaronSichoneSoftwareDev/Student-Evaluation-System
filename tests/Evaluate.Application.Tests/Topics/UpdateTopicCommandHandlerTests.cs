using Evaluate.Application.Topics.Commands.UpdateTopic;
using Evaluate.Application.Tests.Common;
using Xunit;
using CourseEntity = Evaluate.Domain.Entities.Courses.Course;
using TopicEntity = Evaluate.Domain.Entities.Courses.Topic;

namespace Evaluate.Application.Tests.Topics;

public class UpdateTopicCommandHandlerTests
{
    [Fact]
    public async Task Handle_UpdatesNameOrderAndDescription()
    {
        using var context = TestDbContext.Create();
        var course = CourseEntity.Create("MATH01", "Mathematics");
        context.Courses.Add(course);
        await context.SaveChangesAsync(CancellationToken.None);
        var topic = TopicEntity.Create(course.Id, "Algebra", 1);
        context.Topics.Add(topic);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateTopicCommandHandler(RepositoryFactory.Topics(context), context);
        var result = await handler.Handle(new UpdateTopicCommand(topic.Id, "Advanced Algebra", 2, "Covers quadratics."), CancellationToken.None);

        Assert.True(result.Succeeded);
        var updated = context.Topics.Single(t => t.Id == topic.Id);
        Assert.Equal("Advanced Algebra", updated.TopicName);
        Assert.Equal(2, updated.TopicOrder);
        Assert.Equal("Covers quadratics.", updated.Description);
    }

    [Fact]
    public async Task Handle_WithUnknownId_ThrowsNotFound()
    {
        using var context = TestDbContext.Create();
        var handler = new UpdateTopicCommandHandler(RepositoryFactory.Topics(context), context);

        await Assert.ThrowsAsync<Evaluate.Application.Common.Exceptions.NotFoundException>(
            () => handler.Handle(new UpdateTopicCommand(999, "Anything", 1), CancellationToken.None));
    }
}
