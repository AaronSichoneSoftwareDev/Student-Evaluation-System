using Evaluate.Application.Evaluations.Queries.GetStudentReportCard;
using Evaluate.Application.Tests.Common;
using Evaluate.Domain.Enums;
using Evaluate.Infrastructure.Services;
using Xunit;
using AcademicYearEntity = Evaluate.Domain.Entities.Academic.AcademicYear;
using CourseEntity = Evaluate.Domain.Entities.Courses.Course;
using SchoolClassEntity = Evaluate.Domain.Entities.Academic.SchoolClass;
using StudentEntity = Evaluate.Domain.Entities.People.Student;
using StudentEnrollmentEntity = Evaluate.Domain.Entities.People.StudentEnrollment;
using TeacherCourseEntity = Evaluate.Domain.Entities.Courses.TeacherCourse;
using TermEntity = Evaluate.Domain.Entities.Academic.Term;
using TopicEntity = Evaluate.Domain.Entities.Courses.Topic;
using EvaluationEntity = Evaluate.Domain.Entities.Evaluations.Evaluation;

namespace Evaluate.Application.Tests.Evaluations;

public class GetStudentReportCardQueryHandlerTests
{
    [Fact]
    public async Task Handle_GroupsFinalizedEvaluationsByCourse()
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
        var student = StudentEntity.Create("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male);
        context.Students.Add(student);
        await context.SaveChangesAsync(CancellationToken.None);

        context.StudentEnrollments.Add(StudentEnrollmentEntity.Enroll(student.Id, year.Id, schoolClass.Id, new DateOnly(2026, 1, 12)));
        context.TeacherCourses.AddRange(
            TeacherCourseEntity.Assign("teacher-1", math.Id, year.Id, schoolClass.Id),
            TeacherCourseEntity.Assign("teacher-2", english.Id, year.Id, schoolClass.Id));

        var mathEval = EvaluationEntity.Create(student.Id, "teacher-1", math.Id, year.Id, term.Id, new DateOnly(2026, 3, 1));
        mathEval.RecordTopicResult(algebra.Id, 80, "Good progress");
        mathEval.Submit();
        var mathPct = strategy.ComputeFinalPercentage([80]);
        mathEval.Finalize(mathPct, strategy.ComputeGrade(mathPct));

        var englishEval = EvaluationEntity.Create(student.Id, "teacher-2", english.Id, year.Id, term.Id, new DateOnly(2026, 3, 2));
        englishEval.RecordTopicResult(grammar.Id, 65, "Needs work on tenses");
        englishEval.Submit();
        var engPct = strategy.ComputeFinalPercentage([65]);
        englishEval.Finalize(engPct, strategy.ComputeGrade(engPct));

        context.Evaluations.AddRange(mathEval, englishEval);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetStudentReportCardQueryHandler(
            RepositoryFactory.Students(context), RepositoryFactory.Terms(context), RepositoryFactory.AcademicYears(context),
            RepositoryFactory.Enrollments(context), RepositoryFactory.Evaluations(context), RepositoryFactory.Courses(context), RepositoryFactory.TeacherCourses(context));

        var result = await handler.Handle(new GetStudentReportCardQuery(student.Id, term.Id), CancellationToken.None);

        Assert.Equal("Peter Banda", result.StudentName);
        Assert.Equal("Grade 7A", result.ClassName);
        Assert.Equal(2, result.Courses.Count);

        var mathReport = result.Courses.Single(c => c.CourseName == "Mathematics");
        Assert.Single(mathReport.Topics);
        Assert.Equal("Algebra", mathReport.Topics[0].TopicName);
        Assert.Equal(80m, mathReport.Topics[0].Score);
        Assert.Equal("Good progress", mathReport.Topics[0].Comment);

        Assert.True(result.IsComplete);
    }

    [Fact]
    public async Task Handle_WithNoFinalizedEvaluations_ReturnsEmptyCourseList()
    {
        using var context = TestDbContext.Create();
        var student = StudentEntity.Create("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male);
        context.Students.Add(student);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetStudentReportCardQueryHandler(
            RepositoryFactory.Students(context), RepositoryFactory.Terms(context), RepositoryFactory.AcademicYears(context),
            RepositoryFactory.Enrollments(context), RepositoryFactory.Evaluations(context), RepositoryFactory.Courses(context), RepositoryFactory.TeacherCourses(context));

        var result = await handler.Handle(new GetStudentReportCardQuery(student.Id), CancellationToken.None);

        Assert.Empty(result.Courses);
        Assert.False(result.IsComplete);
    }

    [Fact]
    public async Task Handle_WhenOnlySomeRegisteredCoursesAreEvaluated_ReturnsIsCompleteFalse()
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
        context.Topics.Add(algebra);
        var student = StudentEntity.Create("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male);
        context.Students.Add(student);
        await context.SaveChangesAsync(CancellationToken.None);

        context.StudentEnrollments.Add(StudentEnrollmentEntity.Enroll(student.Id, year.Id, schoolClass.Id, new DateOnly(2026, 1, 12)));
        // English is registered to the class but never evaluated for this student.
        context.TeacherCourses.AddRange(
            TeacherCourseEntity.Assign("teacher-1", math.Id, year.Id, schoolClass.Id),
            TeacherCourseEntity.Assign("teacher-2", english.Id, year.Id, schoolClass.Id));

        var mathEval = EvaluationEntity.Create(student.Id, "teacher-1", math.Id, year.Id, term.Id, new DateOnly(2026, 3, 1));
        mathEval.RecordTopicResult(algebra.Id, 80, null);
        mathEval.Submit();
        var mathPct = strategy.ComputeFinalPercentage([80]);
        mathEval.Finalize(mathPct, strategy.ComputeGrade(mathPct));
        context.Evaluations.Add(mathEval);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetStudentReportCardQueryHandler(
            RepositoryFactory.Students(context), RepositoryFactory.Terms(context), RepositoryFactory.AcademicYears(context),
            RepositoryFactory.Enrollments(context), RepositoryFactory.Evaluations(context), RepositoryFactory.Courses(context), RepositoryFactory.TeacherCourses(context));

        var result = await handler.Handle(new GetStudentReportCardQuery(student.Id, term.Id), CancellationToken.None);

        Assert.Single(result.Courses);
        Assert.False(result.IsComplete);
    }
}
