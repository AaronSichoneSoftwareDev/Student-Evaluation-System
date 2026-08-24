using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;

namespace Evaluate.Application.Topics.Commands.UpdateTopic;

public class UpdateTopicCommandHandler(ITopicRepository topics, IUnitOfWork unitOfWork) : IRequestHandler<UpdateTopicCommand, Result>
{
    public async Task<Result> Handle(UpdateTopicCommand request, CancellationToken cancellationToken)
    {
        var topic = await topics.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Courses.Topic), request.Id);

        topic.Update(request.TopicName, request.TopicOrder, request.Description);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
