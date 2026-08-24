using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.Topics.Commands.DeactivateTopic;

[RequirePermission(Permissions.Topics.Edit)]
public record DeactivateTopicCommand(int Id) : IRequest<Result>;
