using MediatR;

namespace Evaluate.Application.Evaluations.Queries.GetPendingEvaluationsList;

public record GetPendingEvaluationsListQuery(int ClassId, string TeacherUserId) : IRequest<PendingEvaluationsResult>;
