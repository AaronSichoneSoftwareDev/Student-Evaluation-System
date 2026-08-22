using Evaluate.Application.Students.Commands.UpdateStudent;
using Evaluate.Application.Tests.Common;
using Evaluate.Domain.Enums;
using Xunit;
using AcademicYearEntity = Evaluate.Domain.Entities.Academic.AcademicYear;
using SchoolClassEntity = Evaluate.Domain.Entities.Academic.SchoolClass;
using StudentEntity = Evaluate.Domain.Entities.People.Student;
using StudentEnrollmentEntity = Evaluate.Domain.Entities.People.StudentEnrollment;

namespace Evaluate.Application.Tests.Students;

public class UpdateStudentCommandHandlerTests
{
    [Fact]
    public async Task Handle_UpdatesStudentFields()
    {
        using var context = TestDbContext.Create();
        var student = StudentEntity.Create("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male);
        context.Students.Add(student);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateStudentCommandHandler(
            RepositoryFactory.Students(context), RepositoryFactory.AcademicYears(context), RepositoryFactory.Enrollments(context), context);
        var command = new UpdateStudentCommand(student.Id, "Peter", "Banda-Mwansa", new DateOnly(2013, 3, 14), Gender.Male, Email: "peter@example.com");

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        var updated = context.Students.Single(s => s.Id == student.Id);
        Assert.Equal("Banda-Mwansa", updated.LastName);
        Assert.Equal("peter@example.com", updated.Email);
    }

    [Fact]
    public async Task Handle_WithNewClassAssignment_CreatesEnrollmentForCurrentYear()
    {
        using var context = TestDbContext.Create();

        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        year.MarkAsCurrent();
        context.AcademicYears.Add(year);
        var classA = SchoolClassEntity.Create("Grade 7A", "Grade 7");
        context.Classes.Add(classA);
        var student = StudentEntity.Create("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male);
        context.Students.Add(student);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateStudentCommandHandler(
            RepositoryFactory.Students(context), RepositoryFactory.AcademicYears(context), RepositoryFactory.Enrollments(context), context);
        var command = new UpdateStudentCommand(student.Id, "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male, ClassId: classA.Id);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        var enrollment = context.StudentEnrollments.Single(e => e.StudentId == student.Id);
        Assert.Equal(classA.Id, enrollment.ClassId);
        Assert.Equal(EnrollmentStatus.Active, enrollment.Status);
    }

    [Fact]
    public async Task Handle_WithDifferentClass_ReassignsExistingEnrollment()
    {
        using var context = TestDbContext.Create();

        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        year.MarkAsCurrent();
        context.AcademicYears.Add(year);
        var classA = SchoolClassEntity.Create("Grade 7A", "Grade 7");
        var classB = SchoolClassEntity.Create("Grade 7B", "Grade 7");
        context.Classes.AddRange(classA, classB);
        var student = StudentEntity.Create("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male);
        context.Students.Add(student);
        await context.SaveChangesAsync(CancellationToken.None);

        context.StudentEnrollments.Add(StudentEnrollmentEntity.Enroll(student.Id, year.Id, classA.Id, new DateOnly(2026, 1, 12)));
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new UpdateStudentCommandHandler(
            RepositoryFactory.Students(context), RepositoryFactory.AcademicYears(context), RepositoryFactory.Enrollments(context), context);
        var command = new UpdateStudentCommand(student.Id, "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male, ClassId: classB.Id);

        await handler.Handle(command, CancellationToken.None);

        var enrollments = context.StudentEnrollments.Where(e => e.StudentId == student.Id).ToList();
        Assert.Single(enrollments);
        Assert.Equal(classB.Id, enrollments[0].ClassId);
    }
}
