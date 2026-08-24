namespace Evaluate.Application.Dashboard;

public record HeaderStatDto(string Label, string Value);

public record StatCardDto(string Label, string Value, string Icon, string DeltaText, bool IsPositive, bool IsAlert = false);

public record DonutSegmentDto(string Label, decimal Value, string Color);

public record DonutChartDto(string Title, string CenterLabel, string CenterValue, string Total1Label, string Total1Value, string Total2Label, string Total2Value, IReadOnlyList<DonutSegmentDto> Segments);

public record ChartSeriesDto(string Name, IReadOnlyList<decimal> Values, string Color);

public record CategoryChartDto(string Title, string Total1Label, string Total1Value, string Total2Label, string Total2Value, IReadOnlyList<string> Categories, IReadOnlyList<ChartSeriesDto> Series);

public record ActivityItemDto(string Date, string Title, string Description, string? Category = null, string? CategoryColor = null);

public record EvaluationRowDto(string StudentName, string Initials, string StatusText, string StatusColor, string Score, string Date, string? FlagText = null, string? FlagColor = null);

public record DashboardViewModel(
    HeaderStatDto HeaderStat1,
    HeaderStatDto HeaderStat2,
    IReadOnlyList<StatCardDto> StatCards,
    DonutChartDto PerformanceDonut,
    CategoryChartDto OutcomesTrend,
    CategoryChartDto BacklogAndRiskOverview,
    IReadOnlyList<ActivityItemDto> ActivityFeed,
    IReadOnlyList<EvaluationRowDto> LatestEvaluations
);
