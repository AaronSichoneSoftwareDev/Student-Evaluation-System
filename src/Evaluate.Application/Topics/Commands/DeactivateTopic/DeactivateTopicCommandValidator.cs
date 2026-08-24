using FluentValidation;

namespace Evaluate.Application.Topics.Commands.DeactivateTopic;

public class DeactivateTopicCommandValidator : AbstractValidator<DeactivateTopicCommand>
{
    public DeactivateTopicCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
