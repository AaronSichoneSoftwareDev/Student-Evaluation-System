using FluentValidation;

namespace Evaluate.Application.Topics.Commands.UpdateTopic;

public class UpdateTopicCommandValidator : AbstractValidator<UpdateTopicCommand>
{
    public UpdateTopicCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.TopicName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TopicOrder).GreaterThanOrEqualTo(0);
    }
}
