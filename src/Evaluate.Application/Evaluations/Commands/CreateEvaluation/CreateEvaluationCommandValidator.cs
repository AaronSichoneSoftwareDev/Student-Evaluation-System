using FluentValidation;

namespace Evaluate.Application.Evaluations.Commands.CreateEvaluation;

public class CreateEvaluationCommandValidator : AbstractValidator<CreateEvaluationCommand>
{
    public CreateEvaluationCommandValidator()
    {
        RuleFor(x => x.StudentId).GreaterThan(0);
        RuleFor(x => x.TeacherUserId).NotEmpty();
        RuleFor(x => x.CourseId).GreaterThan(0);
        RuleFor(x => x.AcademicYearId).GreaterThan(0);
        RuleFor(x => x.TermId).GreaterThan(0);
        RuleFor(x => x.TopicScores).NotEmpty();
        RuleForEach(x => x.TopicScores).ChildRules(score =>
        {
            score.RuleFor(s => s.TopicId).GreaterThan(0);
            score.RuleFor(s => s.Score).InclusiveBetween(0m, 100m);
        });
    }
}
