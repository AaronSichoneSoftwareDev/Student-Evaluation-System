using FluentValidation;

namespace Evaluate.Application.Courses.Commands.DeactivateCourse;

public class DeactivateCourseCommandValidator : AbstractValidator<DeactivateCourseCommand>
{
    public DeactivateCourseCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
