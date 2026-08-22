using Evaluate.Infrastructure.Services;
using Xunit;

namespace Evaluate.Application.Tests.Infrastructure;

public class AverageGradingStrategyTests
{
    private readonly AverageGradingStrategy _strategy = new();

    [Fact]
    public void ComputeFinalPercentage_AveragesTopicScores()
    {
        // (85 + 72 + 90) / 3 = 82.333...
        var scores = new decimal[] { 85, 72, 90 };

        var result = _strategy.ComputeFinalPercentage(scores);

        Assert.Equal(82.33m, decimal.Round(result.Value, 2));
    }

    [Fact]
    public void ComputeFinalPercentage_WithSingleScore_ReturnsThatScore()
    {
        var result = _strategy.ComputeFinalPercentage([77]);

        Assert.Equal(77m, result.Value);
    }

    [Fact]
    public void ComputeFinalPercentage_WithNoScores_ReturnsZero()
    {
        var result = _strategy.ComputeFinalPercentage([]);

        Assert.Equal(0m, result.Value);
    }

    [Theory]
    [InlineData(95, "A")]
    [InlineData(85, "B")]
    [InlineData(75, "C")]
    [InlineData(65, "D")]
    [InlineData(40, "F")]
    public void ComputeGrade_MapsPercentageToLetterGrade(decimal percentage, string expectedGrade)
    {
        var grade = _strategy.ComputeGrade(Domain.Common.ValueObjects.Percentage.Create(percentage));

        Assert.Equal(expectedGrade, grade);
    }
}
