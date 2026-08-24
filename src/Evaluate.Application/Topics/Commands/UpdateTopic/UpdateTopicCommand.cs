using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.Topics.Commands.UpdateTopic;

[RequirePermission(Permissions.Topics.Edit)]
public record UpdateTopicCommand(int Id, string TopicName, int TopicOrder, string? Description = null) : IRequest<Result>;
