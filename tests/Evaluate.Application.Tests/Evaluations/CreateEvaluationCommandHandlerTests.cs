using Evaluate.Application.Evaluations.Commands.CreateEvaluation;
using Evaluate.Application.Tests.Common;
using Evaluate.Domain.Enums;
using Evaluate.Infrastructure.Services;
using Xunit;
using CourseEntity = Evaluate.Domain.Entities.Courses.Course;
using StudentEntity = Evaluate.Domain.Entities.People.Student;
using TopicEntity = Evaluate.Domain.Entities.Courses.Topic;

namespace Evaluate.Application.Tests.Evaluations;

public class CreateEvaluationCommandHandlerTests
{
    private static async Task<(TestDbContext Context, StudentEntity Student, CourseEntity Course, TopicEntity Algebra, TopicEntity Fractions)> SeedAsync()
    {
        var context = TestDbContext.Create();

        var student = StudentEntity.Create("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Gender.Male);
        var course = CourseEntity.Create("MATH01", "Mathematics");
        context.Students.Add(student);
        context.Courses.Add(course);
        await context.SaveChangesAsync(CancellationToken.None);

        var algebra = TopicEntity.Create(course.Id, "Algebra", 1);
        var fractions = TopicEntity.Create(course.Id, "Fractions", 2);
        context.Topics.AddRange(algebra, fractions);
        await context.SaveChangesAsync(CancellationToken.None);

        return (context, student, course, algebra, fractions);
    }

    [Fact]
    public async Task Handle_WithValidTopicScores_CreatesFinalizedEvaluation()
    {
        var (context, student, course, algebra, fractions) = await SeedAsync();
        var handler = new CreateEvaluationCommandHandler(context, new AverageGradingStrategy());

        var command = new CreateEvaluationCommand(
            student.Id, "teacher-1", course.Id, 1, 1, new DateOnly(2026, 3, 20),
            [new TopicScoreInput(algebra.Id, 80, "Good work"), new TopicScoreInput(fractions.Id, 60, "Needs practice")]);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.Succeeded);
        var evaluation = context.Evaluations.Single(e => e.Id == result.Value);
        Assert.Equal(EvaluationStatus.Finalized, evaluation.Status);
        Assert.Equal(70m, evaluation.FinalPercentage);
        Assert.Equal("C", evaluation.FinalGrade);

        var results = context.EvaluationResults.Where(r => r.EvaluationId == evaluation.Id).ToList();
        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.TopicId == algebra.Id && r.Score == 80 && r.Comment == "Good work");
    }

    [Fact]
    public async Task Handle_WithNoTopicScores_ReturnsFailure()
    {
        var (context, student, course, _, _) = await SeedAsync();
        var handler = new CreateEvaluationCommandHandler(context, new AverageGradingStrategy());

        var command = new CreateEvaluationCommand(student.Id, "teacher-1", course.Id, 1, 1, new DateOnly(2026, 3, 20), []);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Handle_WithTopicFromDifferentCourse_ReturnsFailure()
    {
        var (context, student, course, algebra, _) = await SeedAsync();
        var otherCourse = CourseEntity.Create("ENG01", "English");
        context.Courses.Add(otherCourse);
        await context.SaveChangesAsync(CancellationToken.None);
        var otherTopic = TopicEntity.Create(otherCourse.Id, "Grammar", 1);
        context.Topics.Add(otherTopic);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateEvaluationCommandHandler(context, new AverageGradingStrategy());
        var command = new CreateEvaluationCommand(
            student.Id, "teacher-1", course.Id, 1, 1, new DateOnly(2026, 3, 20),
            [new TopicScoreInput(algebra.Id, 80, null), new TopicScoreInput(otherTopic.Id, 70, null)]);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Handle_WhenStudentAlreadyEvaluatedForCourseAndTerm_ReturnsFailure()
    {
        var (context, student, course, algebra, fractions) = await SeedAsync();
        var handler = new CreateEvaluationCommandHandler(context, new AverageGradingStrategy());

        var firstCommand = new CreateEvaluationCommand(
            student.Id, "teacher-1", course.Id, 1, 1, new DateOnly(2026, 3, 20),
            [new TopicScoreInput(algebra.Id, 80, null), new TopicScoreInput(fractions.Id, 60, null)]);
        var firstResult = await handler.Handle(firstCommand, CancellationToken.None);
        Assert.True(firstResult.Succeeded);

        var secondCommand = new CreateEvaluationCommand(
            student.Id, "teacher-1", course.Id, 1, 1, new DateOnly(2026, 3, 21),
            [new TopicScoreInput(algebra.Id, 90, null), new TopicScoreInput(fractions.Id, 70, null)]);
        var secondResult = await handler.Handle(secondCommand, CancellationToken.None);

        Assert.False(secondResult.Succeeded);
    }
}
