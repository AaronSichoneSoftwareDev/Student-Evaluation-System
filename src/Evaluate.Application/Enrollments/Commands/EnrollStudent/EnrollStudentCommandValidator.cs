using FluentValidation;

namespace Evaluate.Application.Enrollments.Commands.EnrollStudent;

public class EnrollStudentCommandValidator : AbstractValidator<EnrollStudentCommand>
{
    public EnrollStudentCommandValidator()
    {
        RuleFor(x => x.StudentId).GreaterThan(0);
        RuleFor(x => x.AcademicYearId).GreaterThan(0);
        RuleFor(x => x.ClassId).GreaterThan(0);
    }
}
