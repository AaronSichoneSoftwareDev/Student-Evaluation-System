namespace Evaluate.Application.Dashboard;

public record HeaderStatDto(string Label, string Value);

public record StatCardDto(string Label, string Value, string Icon, string DeltaText, bool IsPositive);

public record DonutSegmentDto(string Label, decimal Value, string Color);

public record DonutChartDto(string Title, string CenterLabel, string CenterValue, string Total1Label, string Total1Value, string Total2Label, string Total2Value, IReadOnlyList<DonutSegmentDto> Segments);

public record ChartSeriesDto(string Name, IReadOnlyList<decimal> Values, string Color);

public record CategoryChartDto(string Title, string Total1Label, string Total1Value, string Total2Label, string Total2Value, IReadOnlyList<string> Categories, IReadOnlyList<ChartSeriesDto> Series);

public record ActivityItemDto(string Date, string Title, string Description);

public record InboxMessageDto(string SenderName, string Initials, string Preview, string Time);

public record EvaluationRowDto(string StudentName, string Initials, string StatusText, string StatusColor, string Score, string Date);

public record SubmissionRowDto(string ReferenceCode, string StudentName, string Initials, string Subject, string StatusText, string StatusColor, string Score, string Date);

public record QuoteDto(string Text, string InstructorName, string InstructorTitle, string Initials);

public record YearlyStatDto(string Total, string Description, IReadOnlyList<decimal> Values);

public record DashboardViewModel(
    HeaderStatDto HeaderStat1,
    HeaderStatDto HeaderStat2,
    IReadOnlyList<StatCardDto> StatCards,
    DonutChartDto PerformanceDonut,
    CategoryChartDto ScoreTrend,
    CategoryChartDto AssessmentsOverview,
    IReadOnlyList<InboxMessageDto> Inbox,
    IReadOnlyList<ActivityItemDto> ActivityFeed,
    QuoteDto Quote,
    YearlyStatDto YearlyEvaluations,
    IReadOnlyList<EvaluationRowDto> LatestEvaluations,
    IReadOnlyList<SubmissionRowDto> LatestSubmissions
);
