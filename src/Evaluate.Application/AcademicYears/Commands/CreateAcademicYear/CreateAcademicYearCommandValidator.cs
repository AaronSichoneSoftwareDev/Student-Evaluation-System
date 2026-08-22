using FluentValidation;

namespace Evaluate.Application.AcademicYears.Commands.CreateAcademicYear;

public class CreateAcademicYearCommandValidator : AbstractValidator<CreateAcademicYearCommand>
{
    public CreateAcademicYearCommandValidator()
    {
        RuleFor(x => x.YearName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.EndDate).GreaterThan(x => x.StartDate).WithMessage("End date must be after the start date.");
    }
}
