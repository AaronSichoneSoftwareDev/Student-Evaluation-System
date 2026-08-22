using FluentValidation;

namespace Evaluate.Application.Terms.Commands.CreateTerm;

public class CreateTermCommandValidator : AbstractValidator<CreateTermCommand>
{
    public CreateTermCommandValidator()
    {
        RuleFor(x => x.AcademicYearId).GreaterThan(0);
        RuleFor(x => x.TermName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.TermNumber).GreaterThan(0);
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate).WithMessage("End date must be after the start date.");
    }
}
