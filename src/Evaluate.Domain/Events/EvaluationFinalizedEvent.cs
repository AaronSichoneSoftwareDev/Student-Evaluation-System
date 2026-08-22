using Evaluate.Domain.Common;
using Evaluate.Domain.Entities.Evaluations;

namespace Evaluate.Domain.Events;

public class EvaluationFinalizedEvent(Evaluation evaluation) : BaseEvent
{
    public Evaluation Evaluation { get; } = evaluation;
}
