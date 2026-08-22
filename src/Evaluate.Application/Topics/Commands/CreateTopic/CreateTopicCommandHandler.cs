using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using Evaluate.Application.Courses;
using TopicEntity = Evaluate.Domain.Entities.Courses.Topic;
using MediatR;

namespace Evaluate.Application.Topics.Commands.CreateTopic;

public class CreateTopicCommandHandler(ITopicRepository topics, ICourseRepository courses, IUnitOfWork unitOfWork) : IRequestHandler<CreateTopicCommand, Result<int>>
{
    public async Task<Result<int>> Handle(CreateTopicCommand request, CancellationToken cancellationToken)
    {
        var courseExists = await courses.ExistsAsync(request.CourseId, cancellationToken);
        if (!courseExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.Courses.Course), request.CourseId);
        }

        var topic = TopicEntity.Create(request.CourseId, request.TopicName, request.TopicOrder, request.Description);

        topics.Add(topic);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(topic.Id);
    }
}
