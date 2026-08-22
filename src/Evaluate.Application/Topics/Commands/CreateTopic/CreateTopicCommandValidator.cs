using FluentValidation;

namespace Evaluate.Application.Topics.Commands.CreateTopic;

public class CreateTopicCommandValidator : AbstractValidator<CreateTopicCommand>
{
    public CreateTopicCommandValidator()
    {
        RuleFor(x => x.CourseId).GreaterThan(0);
        RuleFor(x => x.TopicName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TopicOrder).GreaterThanOrEqualTo(0);
    }
}
