using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.Evaluations.Commands.CreateEvaluation;

public record TopicScoreInput(int TopicId, decimal Score, string? Comment);

[RequirePermission(Permissions.Evaluations.Create)]
public record CreateEvaluationCommand(
    int StudentId,
    string TeacherUserId,
    int CourseId,
    int AcademicYearId,
    int TermId,
    DateOnly EvaluationDate,
    List<TopicScoreInput> TopicScores,
    string? Comments = null) : IRequest<Result<int>>;
