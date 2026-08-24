using FluentValidation;

namespace Evaluate.Application.AcademicYears.Commands.UpdateAcademicYear;

public class UpdateAcademicYearCommandValidator : AbstractValidator<UpdateAcademicYearCommand>
{
    public UpdateAcademicYearCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.YearName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate).WithMessage("End date must be after the start date.");
    }
}
