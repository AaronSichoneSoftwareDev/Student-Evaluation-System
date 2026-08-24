using FluentValidation;

namespace Evaluate.Application.Terms.Commands.UpdateTerm;

public class UpdateTermCommandValidator : AbstractValidator<UpdateTermCommand>
{
    public UpdateTermCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.TermName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TermNumber).GreaterThan(0);
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate).WithMessage("End date must be after the start date.");
    }
}
