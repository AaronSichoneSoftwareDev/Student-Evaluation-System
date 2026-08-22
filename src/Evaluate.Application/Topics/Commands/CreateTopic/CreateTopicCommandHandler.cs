using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TopicEntity = Evaluate.Domain.Entities.Courses.Topic;

namespace Evaluate.Application.Topics.Commands.CreateTopic;

public class CreateTopicCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateTopicCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateTopicCommand request, CancellationToken cancellationToken)
    {
        var courseExists = await context.Courses.AnyAsync(c => c.Id == request.CourseId, cancellationToken);
        if (!courseExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.Courses.Course), request.CourseId);
        }

        var topic = TopicEntity.Create(request.CourseId, request.TopicName, request.TopicOrder, request.Description);

        context.Topics.Add(topic);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(topic.Id);
    }
}
