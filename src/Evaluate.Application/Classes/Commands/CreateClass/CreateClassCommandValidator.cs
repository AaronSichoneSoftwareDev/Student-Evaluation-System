using FluentValidation;

namespace Evaluate.Application.Classes.Commands.CreateClass;

public class CreateClassCommandValidator : AbstractValidator<CreateClassCommand>
{
    public CreateClassCommandValidator()
    {
        RuleFor(x => x.ClassName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.GradeLevel).NotEmpty().MaximumLength(50);
    }
}
