using Evaluate.Application.Courses.Commands.UpdateCourse;
using Evaluate.Application.Tests.Common;
using Xunit;
using CourseEntity = Evaluate.Domain.Entities.Courses.Course;

namespace Evaluate.Application.Tests.Courses;

public class UpdateCourseCommandHandlerTests
{
    [Fact]
    public async Task Handle_UpdatesNameAndDescription()
    {
        using var context = TestDbContext.Create();
        var course = CourseEntity.Create("HIST01", "History");
        context.Courses.Add(course);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateCourseCommandHandler(RepositoryFactory.Courses(context), context);
        var result = await handler.Handle(new UpdateCourseCommand(course.Id, "World History", "Covers the 20th century onward."), CancellationToken.None);

        Assert.True(result.Succeeded);
        var updated = context.Courses.Single(c => c.Id == course.Id);
        Assert.Equal("World History", updated.CourseName);
        Assert.Equal("Covers the 20th century onward.", updated.Description);
        Assert.Equal("HIST01", updated.CourseCode);
    }

    [Fact]
    public async Task Handle_WithUnknownId_ThrowsNotFound()
    {
        using var context = TestDbContext.Create();
        var handler = new UpdateCourseCommandHandler(RepositoryFactory.Courses(context), context);

        await Assert.ThrowsAsync<Evaluate.Application.Common.Exceptions.NotFoundException>(
            () => handler.Handle(new UpdateCourseCommand(999, "Anything"), CancellationToken.None));
    }
}
