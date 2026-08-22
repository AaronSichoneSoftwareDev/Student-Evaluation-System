using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using EvaluationEntity = Evaluate.Domain.Entities.Evaluations.Evaluation;

namespace Evaluate.Application.Evaluations.Commands.CreateEvaluation;

public class CreateEvaluationCommandHandler(IApplicationDbContext context, IGradingStrategy gradingStrategy) : IRequestHandler<CreateEvaluationCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateEvaluationCommand request, CancellationToken cancellationToken)
    {
        if (request.TopicScores.Count == 0)
        {
            return Result<int>.Failure("At least one topic score is required.");
        }

        var studentExists = await context.Students.AnyAsync(s => s.Id == request.StudentId, cancellationToken);
        if (!studentExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.People.Student), request.StudentId);
        }

        var topicIds = request.TopicScores.Select(s => s.TopicId).ToList();
        var validTopicCount = await context.Topics.CountAsync(t => topicIds.Contains(t.Id) && t.CourseId == request.CourseId, cancellationToken);
        if (validTopicCount != topicIds.Distinct().Count())
        {
            return Result<int>.Failure("One or more topics do not belong to the selected course.");
        }

        var alreadyEvaluated = await context.Evaluations.AnyAsync(
            e => e.StudentId == request.StudentId && e.CourseId == request.CourseId && e.TermId == request.TermId,
            cancellationToken);
        if (alreadyEvaluated)
        {
            return Result<int>.Failure("This student has already been evaluated for this course this term.");
        }

        var evaluation = EvaluationEntity.Create(
            request.StudentId,
            request.TeacherUserId,
            request.CourseId,
            request.AcademicYearId,
            request.TermId,
            request.EvaluationDate,
            request.Comments);

        foreach (var topicScore in request.TopicScores)
        {
            evaluation.RecordTopicResult(topicScore.TopicId, topicScore.Score, topicScore.Comment);
        }

        evaluation.Submit();

        var finalPercentage = gradingStrategy.ComputeFinalPercentage(request.TopicScores.Select(s => s.Score));
        var finalGrade = gradingStrategy.ComputeGrade(finalPercentage);

        evaluation.Finalize(finalPercentage, finalGrade);

        context.Evaluations.Add(evaluation);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(evaluation.Id);
    }
}
