using Evaluate.Application.Common.Interfaces;
using Evaluate.Domain.Common.ValueObjects;

namespace Evaluate.Infrastructure.Services;

/// <summary>Default <see cref="IGradingStrategy"/>: the final percentage is the plain
/// average of every recorded topic score.</summary>
public class AverageGradingStrategy : IGradingStrategy
{
    public Percentage ComputeFinalPercentage(IEnumerable<decimal> topicScores)
    {
        var scores = topicScores.ToList();
        var average = scores.Count == 0 ? 0 : scores.Average();
        return Percentage.Create(average);
    }

    public string ComputeGrade(Percentage percentage) => percentage.Value switch
    {
        >= 90 => "A",
        >= 80 => "B",
        >= 70 => "C",
        >= 60 => "D",
        _ => "F",
    };
}
