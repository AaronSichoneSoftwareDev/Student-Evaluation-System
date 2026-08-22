using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.EvaluationCriteria.Commands.CreateEvaluationCriteria;

[RequirePermission(Permissions.EvaluationCriteria.Create)]
public record CreateEvaluationCriteriaCommand(int CourseId, string CriteriaName, decimal MaxScore, decimal Weight, string? Description = null) : IRequest<Result<int>>;
