using FluentValidation;

namespace Evaluate.Application.EvaluationCriteria.Commands.CreateEvaluationCriteria;

public class CreateEvaluationCriteriaCommandValidator : AbstractValidator<CreateEvaluationCriteriaCommand>
{
    public CreateEvaluationCriteriaCommandValidator()
    {
        RuleFor(x => x.CourseId).GreaterThan(0);
        RuleFor(x => x.CriteriaName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.MaxScore).GreaterThan(0);
        RuleFor(x => x.Weight).InclusiveBetween(0.01m, 100m);
    }
}
