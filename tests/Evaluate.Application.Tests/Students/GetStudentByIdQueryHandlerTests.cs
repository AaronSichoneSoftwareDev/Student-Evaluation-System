using Evaluate.Application.Students.Queries.GetStudentById;
using Evaluate.Application.Tests.Common;
using Evaluate.Domain.Enums;
using Xunit;
using AcademicYearEntity = Evaluate.Domain.Entities.Academic.AcademicYear;
using SchoolClassEntity = Evaluate.Domain.Entities.Academic.SchoolClass;
using StudentEntity = Evaluate.Domain.Entities.People.Student;
using StudentEnrollmentEntity = Evaluate.Domain.Entities.People.StudentEnrollment;

namespace Evaluate.Application.Tests.Students;

public class GetStudentByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithUnknownId_ReturnsNull()
    {
        using var context = TestDbContext.Create();
        var handler = new GetStudentByIdQueryHandler(context);

        var result = await handler.Handle(new GetStudentByIdQuery(999), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_IncludesCurrentYearClassAssignment()
    {
        using var context = TestDbContext.Create();

        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        year.MarkAsCurrent();
        context.AcademicYears.Add(year);
        var schoolClass = SchoolClassEntity.Create("Grade 7A", "Grade 7");
        context.Classes.Add(schoolClass);
        var student = StudentEntity.Create("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male);
        context.Students.Add(student);
        await context.SaveChangesAsync(CancellationToken.None);

        context.StudentEnrollments.Add(StudentEnrollmentEntity.Enroll(student.Id, year.Id, schoolClass.Id, new DateOnly(2026, 1, 12)));
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetStudentByIdQueryHandler(context);

        var result = await handler.Handle(new GetStudentByIdQuery(student.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(schoolClass.Id, result!.ClassId);
        Assert.Equal("Grade 7A", result.ClassName);
    }
}
