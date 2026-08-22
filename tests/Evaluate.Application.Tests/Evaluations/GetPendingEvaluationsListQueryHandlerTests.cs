using Evaluate.Application.Evaluations.Queries.GetPendingEvaluationsList;
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

namespace Evaluate.Application.Tests.Evaluations;

public class GetPendingEvaluationsListQueryHandlerTests
{
    private const string TeacherId = "teacher-1";

    private static async Task<(TestDbContext Context, AcademicYearEntity Year, TermEntity Term, SchoolClassEntity Class, CourseEntity Course, StudentEntity StudentA, StudentEntity StudentB)> SeedBaseDataAsync()
    {
        var context = TestDbContext.Create();

        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        year.MarkAsCurrent();
        context.AcademicYears.Add(year);
        await context.SaveChangesAsync(CancellationToken.None); // year.Id must be real before Term.Create captures it as a FK

        var term = TermEntity.Create(year.Id, "Term 2", 2, new DateOnly(2026, 5, 1), new DateOnly(2026, 8, 1));
        term.MarkAsCurrent();
        var schoolClass = SchoolClassEntity.Create("Grade 7A", "Grade 7");
        var course = CourseEntity.Create("MATH01", "Mathematics");

        context.Terms.Add(term);
        context.Classes.Add(schoolClass);
        context.Courses.Add(course);
        await context.SaveChangesAsync(CancellationToken.None);

        var studentA = StudentEntity.Create("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male);
        var studentB = StudentEntity.Create("STU002", "Mary", "Phiri", new DateOnly(2013, 7, 2), Gender.Female);
        context.Students.AddRange(studentA, studentB);
        await context.SaveChangesAsync(CancellationToken.None);

        context.StudentEnrollments.AddRange(
            StudentEnrollmentEntity.Enroll(studentA.Id, year.Id, schoolClass.Id, new DateOnly(2026, 1, 12)),
            StudentEnrollmentEntity.Enroll(studentB.Id, year.Id, schoolClass.Id, new DateOnly(2026, 1, 12)));
        context.TeacherCourses.Add(TeacherCourseEntity.Assign(TeacherId, course.Id, year.Id, schoolClass.Id));
        await context.SaveChangesAsync(CancellationToken.None);

        return (context, year, term, schoolClass, course, studentA, studentB);
    }

    [Fact]
    public async Task Handle_WithNoEvaluationsYet_ReturnsAllEnrolledStudentsAsPending()
    {
        var (context, _, _, schoolClass, _, studentA, studentB) = await SeedBaseDataAsync();
        var handler = new GetPendingEvaluationsListQueryHandler(context);

        var result = await handler.Handle(new GetPendingEvaluationsListQuery(schoolClass.Id, TeacherId), CancellationToken.None);

        Assert.True(result.HasCurrentTerm);
        Assert.True(result.TeacherAssigned);
        Assert.Equal(2, result.Students.Count);
        Assert.Contains(result.Students, s => s.StudentId == studentA.Id);
        Assert.Contains(result.Students, s => s.StudentId == studentB.Id);
    }

    [Fact]
    public async Task Handle_ExcludesStudentsAlreadyEvaluatedForCurrentTerm()
    {
        var (context, year, term, schoolClass, course, studentA, studentB) = await SeedBaseDataAsync();

        var topic = TopicEntity.Create(course.Id, "Algebra", 1);
        context.Topics.Add(topic);
        await context.SaveChangesAsync(CancellationToken.None);

        var evaluation = EvaluationEntity.Create(studentA.Id, TeacherId, course.Id, year.Id, term.Id, new DateOnly(2026, 6, 1));
        evaluation.RecordTopicResult(topic.Id, 80, null);
        evaluation.Submit();
        var strategy = new AverageGradingStrategy();
        var percentage = strategy.ComputeFinalPercentage([80]);
        evaluation.Finalize(percentage, strategy.ComputeGrade(percentage));
        context.Evaluations.Add(evaluation);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetPendingEvaluationsListQueryHandler(context);

        var result = await handler.Handle(new GetPendingEvaluationsListQuery(schoolClass.Id, TeacherId), CancellationToken.None);

        Assert.Single(result.Students);
        Assert.Equal(studentB.Id, result.Students[0].StudentId);
    }

    [Fact]
    public async Task Handle_WithTeacherAssignedToMultipleSubjects_ListsAllSubjectsAndStillPendingIfAnyIncomplete()
    {
        var (context, year, term, schoolClass, mathCourse, studentA, studentB) = await SeedBaseDataAsync();

        var englishCourse = CourseEntity.Create("ENG01", "English");
        context.Courses.Add(englishCourse);
        await context.SaveChangesAsync(CancellationToken.None);
        context.TeacherCourses.Add(TeacherCourseEntity.Assign(TeacherId, englishCourse.Id, year.Id, schoolClass.Id));

        var mathTopic = TopicEntity.Create(mathCourse.Id, "Algebra", 1);
        context.Topics.Add(mathTopic);
        await context.SaveChangesAsync(CancellationToken.None);

        // Student A is finalized in Math but not in English — still pending overall.
        var strategy = new AverageGradingStrategy();
        var mathEval = EvaluationEntity.Create(studentA.Id, TeacherId, mathCourse.Id, year.Id, term.Id, new DateOnly(2026, 6, 1));
        mathEval.RecordTopicResult(mathTopic.Id, 80, null);
        mathEval.Submit();
        var percentage = strategy.ComputeFinalPercentage([80]);
        mathEval.Finalize(percentage, strategy.ComputeGrade(percentage));
        context.Evaluations.Add(mathEval);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetPendingEvaluationsListQueryHandler(context);

        var result = await handler.Handle(new GetPendingEvaluationsListQuery(schoolClass.Id, TeacherId), CancellationToken.None);

        Assert.Equal(2, result.AvailableCourses.Count);
        Assert.Contains(result.AvailableCourses, c => c.CourseName == "Mathematics");
        Assert.Contains(result.AvailableCourses, c => c.CourseName == "English");

        // Both students still pending: A needs English, B needs both.
        Assert.Equal(2, result.Students.Count);
    }

    [Fact]
    public async Task Handle_WithTeacherNotAssignedToClass_ReturnsTeacherAssignedFalse()
    {
        var (context, _, _, schoolClass, _, _, _) = await SeedBaseDataAsync();
        var handler = new GetPendingEvaluationsListQueryHandler(context);

        var result = await handler.Handle(new GetPendingEvaluationsListQuery(schoolClass.Id, "someone-else"), CancellationToken.None);

        Assert.True(result.HasCurrentTerm);
        Assert.False(result.TeacherAssigned);
        Assert.Empty(result.Students);
    }

    [Fact]
    public async Task Handle_WithNoCurrentAcademicYear_ReturnsHasCurrentTermFalse()
    {
        using var context = TestDbContext.Create();
        var handler = new GetPendingEvaluationsListQueryHandler(context);

        var result = await handler.Handle(new GetPendingEvaluationsListQuery(1, TeacherId), CancellationToken.None);

        Assert.False(result.HasCurrentTerm);
        Assert.Empty(result.Students);
    }
}
