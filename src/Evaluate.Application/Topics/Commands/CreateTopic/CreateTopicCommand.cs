using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.Topics.Commands.CreateTopic;

[RequirePermission(Permissions.Topics.Create)]
public record CreateTopicCommand(int CourseId, string TopicName, int TopicOrder, string? Description = null) : IRequest<Result<int>>;
