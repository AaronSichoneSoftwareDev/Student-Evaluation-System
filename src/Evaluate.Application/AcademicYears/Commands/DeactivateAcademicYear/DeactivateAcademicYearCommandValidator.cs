using FluentValidation;

namespace Evaluate.Application.AcademicYears.Commands.DeactivateAcademicYear;

public class DeactivateAcademicYearCommandValidator : AbstractValidator<DeactivateAcademicYearCommand>
{
    public DeactivateAcademicYearCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
