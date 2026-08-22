using Evaluate.Domain.Common;
using Evaluate.Domain.Entities.People;

namespace Evaluate.Domain.Events;

public class StudentEnrolledEvent(StudentEnrollment enrollment) : BaseEvent
{
    public StudentEnrollment Enrollment { get; } = enrollment;
}
