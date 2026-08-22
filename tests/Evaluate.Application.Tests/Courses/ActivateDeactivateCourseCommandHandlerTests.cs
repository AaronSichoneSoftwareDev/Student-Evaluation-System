using Evaluate.Application.Courses.Commands.ActivateCourse;
using Evaluate.Application.Courses.Commands.DeactivateCourse;
using Evaluate.Application.Tests.Common;
using Xunit;
using CourseEntity = Evaluate.Domain.Entities.Courses.Course;

namespace Evaluate.Application.Tests.Courses;

public class ActivateDeactivateCourseCommandHandlerTests
{
    [Fact]
    public async Task Deactivate_MarksCourseInactive()
    {
        using var context = TestDbContext.Create();
        var course = CourseEntity.Create("HIST01", "History");
        context.Courses.Add(course);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new DeactivateCourseCommandHandler(RepositoryFactory.Courses(context), context);
        var result = await handler.Handle(new DeactivateCourseCommand(course.Id), CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.False(context.Courses.Single(c => c.Id == course.Id).IsActive);
    }

    [Fact]
    public async Task Activate_MarksCourseActiveAgain()
    {
        using var context = TestDbContext.Create();
        var course = CourseEntity.Create("HIST01", "History");
        course.Deactivate();
        context.Courses.Add(course);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new ActivateCourseCommandHandler(RepositoryFactory.Courses(context), context);
        var result = await handler.Handle(new ActivateCourseCommand(course.Id), CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.True(context.Courses.Single(c => c.Id == course.Id).IsActive);
    }
}
