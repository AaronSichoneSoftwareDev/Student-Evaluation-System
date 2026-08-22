using FluentValidation;

namespace Evaluate.Application.Students.Commands.DeactivateStudent;

public class DeactivateStudentCommandValidator : AbstractValidator<DeactivateStudentCommand>
{
    public DeactivateStudentCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
