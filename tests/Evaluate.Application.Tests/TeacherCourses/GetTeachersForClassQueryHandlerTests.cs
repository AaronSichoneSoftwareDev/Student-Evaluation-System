using Evaluate.Application.TeacherCourses.Queries.GetTeachersForClass;
using Evaluate.Application.Tests.Common;
using Xunit;
using AcademicYearEntity = Evaluate.Domain.Entities.Academic.AcademicYear;
using CourseEntity = Evaluate.Domain.Entities.Courses.Course;
using SchoolClassEntity = Evaluate.Domain.Entities.Academic.SchoolClass;
using TeacherCourseEntity = Evaluate.Domain.Entities.Courses.TeacherCourse;

namespace Evaluate.Application.Tests.TeacherCourses;

public class GetTeachersForClassQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsOnlyTeachersAssignedToThatClass()
    {
        using var context = TestDbContext.Create();

        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        year.MarkAsCurrent();
        context.AcademicYears.Add(year);
        var classA = SchoolClassEntity.Create("Grade 7A", "Grade 7");
        var classB = SchoolClassEntity.Create("Grade 8A", "Grade 8");
        var math = CourseEntity.Create("MATH01", "Mathematics");
        var science = CourseEntity.Create("SCI01", "Science");
        context.Classes.AddRange(classA, classB);
        context.Courses.AddRange(math, science);
        await context.SaveChangesAsync(CancellationToken.None);

        context.TeacherCourses.AddRange(
            TeacherCourseEntity.Assign("teacher-1", math.Id, year.Id, classA.Id),
            TeacherCourseEntity.Assign("teacher-2", science.Id, year.Id, classB.Id));
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetTeachersForClassQueryHandler(
            RepositoryFactory.TeacherCourses(context), RepositoryFactory.AcademicYears(context), new FakeIdentityService());

        var result = await handler.Handle(new GetTeachersForClassQuery(classA.Id), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("teacher-1", result[0].Id);
    }

    [Fact]
    public async Task Handle_WithNoAssignmentsForClass_ReturnsEmpty()
    {
        using var context = TestDbContext.Create();

        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        year.MarkAsCurrent();
        var classA = SchoolClassEntity.Create("Grade 7A", "Grade 7");
        context.AcademicYears.Add(year);
        context.Classes.Add(classA);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetTeachersForClassQueryHandler(
            RepositoryFactory.TeacherCourses(context), RepositoryFactory.AcademicYears(context), new FakeIdentityService());

        var result = await handler.Handle(new GetTeachersForClassQuery(classA.Id), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithNoCurrentAcademicYear_ReturnsEmpty()
    {
        using var context = TestDbContext.Create();
        var handler = new GetTeachersForClassQueryHandler(
            RepositoryFactory.TeacherCourses(context), RepositoryFactory.AcademicYears(context), new FakeIdentityService());

        var result = await handler.Handle(new GetTeachersForClassQuery(1), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_DoesNotDuplicateTeacherAssignedToMultipleSubjectsInTheClass()
    {
        using var context = TestDbContext.Create();

        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        year.MarkAsCurrent();
        var classA = SchoolClassEntity.Create("Grade 7A", "Grade 7");
        var math = CourseEntity.Create("MATH01", "Mathematics");
        var english = CourseEntity.Create("ENG01", "English");
        context.AcademicYears.Add(year);
        context.Classes.Add(classA);
        context.Courses.AddRange(math, english);
        await context.SaveChangesAsync(CancellationToken.None);

        context.TeacherCourses.AddRange(
            TeacherCourseEntity.Assign("teacher-1", math.Id, year.Id, classA.Id),
            TeacherCourseEntity.Assign("teacher-1", english.Id, year.Id, classA.Id));
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetTeachersForClassQueryHandler(
            RepositoryFactory.TeacherCourses(context), RepositoryFactory.AcademicYears(context), new FakeIdentityService());

        var result = await handler.Handle(new GetTeachersForClassQuery(classA.Id), CancellationToken.None);

        Assert.Single(result);
    }
}
