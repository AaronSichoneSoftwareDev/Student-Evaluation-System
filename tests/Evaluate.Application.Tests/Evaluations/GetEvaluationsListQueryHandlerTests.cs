using Evaluate.Application.Evaluations.Queries.GetEvaluationsList;
using Evaluate.Application.Tests.Common;
using Xunit;
using EvaluationEntity = Evaluate.Domain.Entities.Evaluations.Evaluation;
using StudentEntity = Evaluate.Domain.Entities.People.Student;

namespace Evaluate.Application.Tests.Evaluations;

public class GetEvaluationsListQueryHandlerTests
{
    [Fact]
    public async Task Handle_FiltersByStudentId_UsingSpecification()
    {
        using var context = TestDbContext.Create();

        var studentA = StudentEntity.Create("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Domain.Enums.Gender.Male);
        var studentB = StudentEntity.Create("STU002", "Mary", "Phiri", new DateOnly(2013, 7, 2), Domain.Enums.Gender.Female);
        context.Students.AddRange(studentA, studentB);
        await context.SaveChangesAsync(CancellationToken.None);

        var evaluationForA = EvaluationEntity.Create(studentA.Id, "teacher-1", 1, 1, 1, new DateOnly(2026, 3, 1));
        var evaluationForB = EvaluationEntity.Create(studentB.Id, "teacher-1", 1, 1, 1, new DateOnly(2026, 3, 2));
        context.Evaluations.AddRange(evaluationForA, evaluationForB);
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetEvaluationsListQueryHandler(context);

        var result = await handler.Handle(new GetEvaluationsListQuery(StudentId: studentA.Id), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal(studentA.Id, result[0].StudentId);
    }

    [Fact]
    public async Task Handle_WithNoFilters_ReturnsAllEvaluations()
    {
        using var context = TestDbContext.Create();

        var student = StudentEntity.Create("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Domain.Enums.Gender.Male);
        context.Students.Add(student);
        await context.SaveChangesAsync(CancellationToken.None);

        context.Evaluations.AddRange(
            EvaluationEntity.Create(student.Id, "teacher-1", 1, 1, 1, new DateOnly(2026, 3, 1)),
            EvaluationEntity.Create(student.Id, "teacher-1", 2, 1, 1, new DateOnly(2026, 3, 2)));
        await context.SaveChangesAsync(CancellationToken.None);

        var handler = new GetEvaluationsListQueryHandler(context);

        var result = await handler.Handle(new GetEvaluationsListQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
    }
}
