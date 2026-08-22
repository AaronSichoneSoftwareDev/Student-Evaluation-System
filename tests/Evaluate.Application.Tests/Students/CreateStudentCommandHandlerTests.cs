using Evaluate.Application.Students.Commands.CreateStudent;
using Evaluate.Application.Tests.Common;
using Evaluate.Domain.Enums;
using Xunit;
using AcademicYearEntity = Evaluate.Domain.Entities.Academic.AcademicYear;
using SchoolClassEntity = Evaluate.Domain.Entities.Academic.SchoolClass;

namespace Evaluate.Application.Tests.Students;

public class CreateStudentCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_CreatesStudentAndReturnsId()
    {
        using var context = TestDbContext.Create();
        var handler = new CreateStudentCommandHandler(context);
        var command = new CreateStudentCommand("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.True(result.Value > 0);
        Assert.Equal(1, context.Students.Count());
    }

    [Fact]
    public async Task Handle_WithDuplicateStudentNumber_ReturnsFailure()
    {
        using var context = TestDbContext.Create();
        var handler = new CreateStudentCommandHandler(context);
        var command = new CreateStudentCommand("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male);
        await handler.Handle(command, CancellationToken.None);

        var result = await handler.Handle(command with { FirstName = "Someone Else" }, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Equal(1, context.Students.Count());
    }

    [Fact]
    public async Task Handle_WithClassId_EnrollsStudentInCurrentAcademicYear()
    {
        using var context = TestDbContext.Create();
        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        year.MarkAsCurrent();
        context.AcademicYears.Add(year);
        var schoolClass = SchoolClassEntity.Create("Grade 7A", "Grade 7");
        context.Classes.Add(schoolClass);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateStudentCommandHandler(context);
        var command = new CreateStudentCommand("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male, ClassId: schoolClass.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        var enrollment = context.StudentEnrollments.Single(e => e.StudentId == result.Value);
        Assert.Equal(schoolClass.Id, enrollment.ClassId);
        Assert.Equal(year.Id, enrollment.AcademicYearId);
    }

    [Fact]
    public async Task Handle_WithClassIdButNoCurrentYear_StillCreatesStudentWithoutEnrollment()
    {
        using var context = TestDbContext.Create();
        var schoolClass = SchoolClassEntity.Create("Grade 7A", "Grade 7");
        context.Classes.Add(schoolClass);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateStudentCommandHandler(context);
        var command = new CreateStudentCommand("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male, ClassId: schoolClass.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Empty(context.StudentEnrollments);
    }
}
