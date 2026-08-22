using FluentValidation;

namespace Evaluate.Application.Courses.Commands.ActivateCourse;

public class ActivateCourseCommandValidator : AbstractValidator<ActivateCourseCommand>
{
    public ActivateCourseCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
