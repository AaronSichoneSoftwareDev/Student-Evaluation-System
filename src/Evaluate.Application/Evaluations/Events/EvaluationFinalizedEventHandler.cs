using Evaluate.Application.Common.Models;
using Evaluate.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Evaluate.Application.Evaluations.Events;

public class EvaluationFinalizedEventHandler(ILogger<EvaluationFinalizedEventHandler> logger) : INotificationHandler<DomainEventNotification<EvaluationFinalizedEvent>>
{
    public Task Handle(DomainEventNotification<EvaluationFinalizedEvent> notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Evaluation {EvaluationId} finalized for student {StudentId} with grade {Grade} ({Percentage}%)",
            notification.DomainEvent.Evaluation.Id,
            notification.DomainEvent.Evaluation.StudentId,
            notification.DomainEvent.Evaluation.FinalGrade,
            notification.DomainEvent.Evaluation.FinalPercentage);

        return Task.CompletedTask;
    }
}
