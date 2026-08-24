using FluentValidation;

namespace Evaluate.Application.Terms.Commands.DeactivateTerm;

public class DeactivateTermCommandValidator : AbstractValidator<DeactivateTermCommand>
{
    public DeactivateTermCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
