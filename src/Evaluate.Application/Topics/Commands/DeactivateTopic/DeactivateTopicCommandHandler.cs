using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;

namespace Evaluate.Application.Topics.Commands.DeactivateTopic;

public class DeactivateTopicCommandHandler(ITopicRepository topics, IUnitOfWork unitOfWork) : IRequestHandler<DeactivateTopicCommand, Result>
{
    public async Task<Result> Handle(DeactivateTopicCommand request, CancellationToken cancellationToken)
    {
        var topic = await topics.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Courses.Topic), request.Id);

        topic.Deactivate();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
