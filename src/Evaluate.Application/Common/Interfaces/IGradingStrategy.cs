using Evaluate.Domain.Common.ValueObjects;

namespace Evaluate.Application.Common.Interfaces;

/// <summary>Strategy pattern: how a set of per-topic scores becomes one final percentage +
/// letter grade. Swappable (e.g. simple average vs. weighted by topic difficulty) without
/// touching the command handler that calls it.</summary>
public interface IGradingStrategy
{
    Percentage ComputeFinalPercentage(IEnumerable<decimal> topicScores);

    string ComputeGrade(Percentage percentage);
}
