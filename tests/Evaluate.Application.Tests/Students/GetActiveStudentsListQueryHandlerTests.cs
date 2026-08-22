using Evaluate.Application.Students.Commands.DeactivateStudent;
using Evaluate.Application.Students.Queries.GetActiveStudentsList;
using Evaluate.Application.Tests.Common;
using Evaluate.Domain.Enums;
using Evaluate.Infrastructure.Services;
using Xunit;
using AcademicYearEntity = Evaluate.Domain.Entities.Academic.AcademicYear;
using CourseEntity = Evaluate.Domain.Entities.Courses.Course;
using EvaluationEntity = Evaluate.Domain.Entities.Evaluations.Evaluation;
using SchoolClassEntity = Evaluate.Domain.Entities.Academic.SchoolClass;
using StudentEntity = Evaluate.Domain.Entities.People.Student;
using StudentEnrollmentEntity = Evaluate.Domain.Entities.People.StudentEnrollment;
using TeacherCourseEntity = Evaluate.Domain.Entities.Courses.TeacherCourse;
using TermEntity = Evaluate.Domain.Entities.Academic.Term;
using TopicEntity = Evaluate.Domain.Entities.Courses.Topic;

namespace Evaluate.Application.Tests.Students;

public class GetActiveStudentsListQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithNoCurrentAcademicYear_ReturnsEmptyList()
    {
        using var context = TestDbContext.Create();
        var handler = new GetActiveStudentsListQueryHandler(context);

        var result = await handler.Handle(new GetActiveStudentsListQuery(), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_OnlyReturnsActivelyEnrolledStudentsForCurrentYear()
    {
        using var context = TestDbContext.Create();

        var currentYear = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        currentYear.MarkAsCurrent();
        var pastYear = AcademicYearEntity.Create("2025", new DateOnly(2025, 1, 1), new DateOnly(2025, 12, 31));
        var schoolClass = SchoolClassEntity.Create("Grade 7A", "Grade 7");
        context.AcademicYears.AddRange(currentYear, pastYear);
        context.Classes.Add(schoolClass);
        await context.SaveChangesAsync(CancellationToken.None);

        var activeStudent = StudentEntity.Create("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male);
        var withdrawnStudent = StudentEntity.Create("STU002", "Mary", "Phiri", new DateOnly(2013, 7, 2), Gender.Female);
        var pastYearStudent = StudentEntity.Create("STU003", "Chanda", "Mwape", new DateOnly(2012, 11, 20), Gender.Female);
        context.Students.AddRange(activeStudent, withdrawnStudent, pastYearStudent);
        await context.SaveChangesAsync(CancellationToken.None);

        var withdrawnEnrollment = StudentEnrollmentEntity.Enroll(withdrawnStudent.Id, currentYear.Id, schoolClass.Id, new DateOnly(2026, 1, 12));
        withdrawnEnrollment.Withdraw();

        context.StudentEnrollments.AddRange(
            StudentEnrollmentEntity.Enroll(activeStudent.Id, currentYear.Id, schoolClass.Id, new DateOnly(2026, 1, 12)),
            withdrawnEnrollment,
            StudentEnrollmentEntity.Enroll(pastYearStudent.Id, pastYear.Id, schoolClass.Id, new DateOnly(2025, 1, 12)));
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetActiveStudentsListQueryHandler(context);

        var result = await handler.Handle(new GetActiveStudentsListQuery(), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(activeStudent.Id, result[0].StudentId);
    }

    [Fact]
    public async Task Handle_ExcludesStudentAfterDeactivation()
    {
        using var context = TestDbContext.Create();

        var currentYear = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        currentYear.MarkAsCurrent();
        var schoolClass = SchoolClassEntity.Create("Grade 7A", "Grade 7");
        context.AcademicYears.Add(currentYear);
        context.Classes.Add(schoolClass);
        var student = StudentEntity.Create("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male);
        context.Students.Add(student);
        await context.SaveChangesAsync(CancellationToken.None);

        context.StudentEnrollments.Add(StudentEnrollmentEntity.Enroll(student.Id, currentYear.Id, schoolClass.Id, new DateOnly(2026, 1, 12)));
        await context.SaveChangesAsync(CancellationToken.None);

        var listHandler = new GetActiveStudentsListQueryHandler(context);
        Assert.Single(await listHandler.Handle(new GetActiveStudentsListQuery(), CancellationToken.None));

        var deactivateHandler = new DeactivateStudentCommandHandler(context);
        await deactivateHandler.Handle(new DeactivateStudentCommand(student.Id), CancellationToken.None);

        var result = await listHandler.Handle(new GetActiveStudentsListQuery(), CancellationToken.None);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_SetsReportCardReadyOnlyWhenEveryRegisteredCourseIsFinalized()
    {
        using var context = TestDbContext.Create();
        var strategy = new AverageGradingStrategy();

        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        year.MarkAsCurrent();
        context.AcademicYears.Add(year);
        await context.SaveChangesAsync(CancellationToken.None);

        var term = TermEntity.Create(year.Id, "Term 1", 1, new DateOnly(2026, 1, 1), new DateOnly(2026, 4, 1));
        term.MarkAsCurrent();
        var schoolClass = SchoolClassEntity.Create("Grade 7A", "Grade 7");
        var math = CourseEntity.Create("MATH01", "Mathematics");
        var english = CourseEntity.Create("ENG01", "English");
        context.Terms.Add(term);
        context.Classes.Add(schoolClass);
        context.Courses.AddRange(math, english);
        await context.SaveChangesAsync(CancellationToken.None);

        var algebra = TopicEntity.Create(math.Id, "Algebra", 1);
        var grammar = TopicEntity.Create(english.Id, "Grammar", 1);
        context.Topics.AddRange(algebra, grammar);

        var readyStudent = StudentEntity.Create("STU010", "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male);
        var notReadyStudent = StudentEntity.Create("STU011", "Mary", "Phiri", new DateOnly(2013, 7, 2), Gender.Female);
        context.Students.AddRange(readyStudent, notReadyStudent);
        await context.SaveChangesAsync(CancellationToken.None);

        context.StudentEnrollments.AddRange(
            StudentEnrollmentEntity.Enroll(readyStudent.Id, year.Id, schoolClass.Id, new DateOnly(2026, 1, 12)),
            StudentEnrollmentEntity.Enroll(notReadyStudent.Id, year.Id, schoolClass.Id, new DateOnly(2026, 1, 12)));
        context.TeacherCourses.AddRange(
            TeacherCourseEntity.Assign("teacher-1", math.Id, year.Id, schoolClass.Id),
            TeacherCourseEntity.Assign("teacher-2", english.Id, year.Id, schoolClass.Id));

        foreach (var student in new[] { readyStudent, notReadyStudent })
        {
            var mathEval = EvaluationEntity.Create(student.Id, "teacher-1", math.Id, year.Id, term.Id, new DateOnly(2026, 3, 1));
            mathEval.RecordTopicResult(algebra.Id, 80, null);
            mathEval.Submit();
            var pct = strategy.ComputeFinalPercentage([80]);
            mathEval.Finalize(pct, strategy.ComputeGrade(pct));
            context.Evaluations.Add(mathEval);
        }

        // Only readyStudent also has a finalized English evaluation — notReadyStudent is still missing one.
        var englishEval = EvaluationEntity.Create(readyStudent.Id, "teacher-2", english.Id, year.Id, term.Id, new DateOnly(2026, 3, 2));
        englishEval.RecordTopicResult(grammar.Id, 70, null);
        englishEval.Submit();
        var englishPct = strategy.ComputeFinalPercentage([70]);
        englishEval.Finalize(englishPct, strategy.ComputeGrade(englishPct));
        context.Evaluations.Add(englishEval);

        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetActiveStudentsListQueryHandler(context);
        var result = await handler.Handle(new GetActiveStudentsListQuery(), CancellationToken.None);

        Assert.True(result.Single(s => s.StudentId == readyStudent.Id).ReportCardReady);
        Assert.False(result.Single(s => s.StudentId == notReadyStudent.Id).ReportCardReady);
    }
}
