using Evaluate.Application.Common.Models;
using Evaluate.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Evaluate.Application.Enrollments.Events;

/// <summary>Observer pattern: reacts to a student being enrolled without the
/// enrollment command handler knowing this handler exists.</summary>
public class StudentEnrolledEventHandler(ILogger<StudentEnrolledEventHandler> logger) : INotificationHandler<DomainEventNotification<StudentEnrolledEvent>>
{
    public Task Handle(DomainEventNotification<StudentEnrolledEvent> notification, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Student {StudentId} enrolled in class {ClassId} for academic year {AcademicYearId}",
            notification.DomainEvent.Enrollment.StudentId,
            notification.DomainEvent.Enrollment.ClassId,
            notification.DomainEvent.Enrollment.AcademicYearId);

        return Task.CompletedTask;
    }
}
