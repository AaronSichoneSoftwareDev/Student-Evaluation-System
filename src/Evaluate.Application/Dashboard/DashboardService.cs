using System.Globalization;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Domain.Entities.Dashboard;
using Evaluate.Domain.Enums.Dashboard;

namespace Evaluate.Application.Dashboard;

public class DashboardService(
    IStudentRepository studentRepository,
    IEvaluationRepository evaluationRepository,
    ISubmissionRepository submissionRepository,
    IActivityFeedRepository activityFeedRepository,
    IInboxRepository inboxRepository,
    IInstructorRepository instructorRepository) : IDashboardService
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    public async Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var students = await studentRepository.GetAllAsync(cancellationToken);
        var evaluations = await evaluationRepository.GetAllAsync(cancellationToken);
        var submissions = await submissionRepository.GetAllAsync(cancellationToken);
        var latestEvaluations = await evaluationRepository.GetLatestAsync(5, cancellationToken);
        var latestSubmissions = await submissionRepository.GetLatestAsync(6, cancellationToken);
        var activity = await activityFeedRepository.GetRecentAsync(5, cancellationToken);
        var inbox = await inboxRepository.GetAllAsync(cancellationToken);
        var instructor = await instructorRepository.GetFeaturedAsync(cancellationToken);

        var averageScore = evaluations.Count > 0 ? evaluations.Average(e => e.Score) : 0m;
        var passedCount = evaluations.Count(e => e.Status == EvaluationStatus.Passed);
        var pendingCount = evaluations.Count(e => e.Status == EvaluationStatus.Pending);
        var failedCount = evaluations.Count(e => e.Status == EvaluationStatus.Failed);
        var passRate = evaluations.Count > 0 ? (decimal)passedCount / evaluations.Count * 100 : 0m;
        var evaluationsCompleted = evaluations.Count(e => e.Status != EvaluationStatus.Pending);

        var statCards = new List<StatCardDto>
        {
            new("Students Evaluated", students.Count.ToString("N0", Culture), "bi-people-fill", "+12%", true),
            new("Average Score", $"{averageScore.ToString("0.0", Culture)}%", "bi-graph-up-arrow", "+4%", true),
            new("Pass Rate", $"{passRate.ToString("0", Culture)}%", "bi-award-fill", "+8%", true),
            new("Evaluations Completed", evaluationsCompleted.ToString("N0", Culture), "bi-clipboard-check-fill", "-2%", false),
        };

        var donut = new DonutChartDto(
            "Overall Performance",
            "Pass Rate",
            $"{passRate.ToString("0", Culture)}%",
            "Passed",
            passedCount.ToString("N0", Culture),
            "Needs Review",
            (pendingCount + failedCount).ToString("N0", Culture),
            new List<DonutSegmentDto>
            {
                new("Passed", passedCount, "#6f63d9"),
                new("Pending", pendingCount, "#55c6e8"),
                new("Failed", failedCount, "#e2e6f1"),
            });

        var years = evaluations.Select(e => e.Date.Year).Distinct().OrderBy(y => y).ToList();
        if (years.Count == 0) years = [DateTime.UtcNow.Year];

        var highest = years.Select(y => evaluations.Where(e => e.Date.Year == y).Select(e => e.Score).DefaultIfEmpty(0).Max()).ToList();
        var average = years.Select(y => evaluations.Where(e => e.Date.Year == y).Select(e => e.Score).DefaultIfEmpty(0).Average()).ToList();
        var lowest = years.Select(y => evaluations.Where(e => e.Date.Year == y).Select(e => e.Score).DefaultIfEmpty(0).Min()).ToList();

        var scoreTrend = new CategoryChartDto(
            "Evaluation Score Trend",
            "Marketplace",
            $"{highest.DefaultIfEmpty(0).Max().ToString("0", Culture)}",
            "Total Income",
            $"{average.DefaultIfEmpty(0).Average().ToString("0", Culture)}",
            years.Select(y => y.ToString(Culture)).ToList(),
            new List<ChartSeriesDto>
            {
                new("Highest Score", highest, "#c9c3f7"),
                new("Average Score", average, "#55c6e8"),
                new("Lowest Score", lowest, "#6f63d9"),
            });

        var subYears = submissions.Select(s => s.Date.Year).Distinct().OrderBy(y => y).ToList();
        if (subYears.Count == 0) subYears = [DateTime.UtcNow.Year];
        var graded = subYears.Select(y => (decimal)submissions.Count(s => s.Date.Year == y && s.Status == SubmissionStatus.Graded)).ToList();
        var pending = subYears.Select(y => (decimal)submissions.Count(s => s.Date.Year == y && s.Status != SubmissionStatus.Graded)).ToList();

        var assessmentsOverview = new CategoryChartDto(
            "Assessments Overview",
            "This Term",
            submissions.Count.ToString("N0", Culture),
            "Last Term",
            Math.Max(0, submissions.Count - 12).ToString("N0", Culture),
            subYears.Select(y => y.ToString(Culture)).ToList(),
            new List<ChartSeriesDto>
            {
                new("Graded", graded, "#6f63d9"),
                new("Pending", pending, "#c9c3f7"),
            });

        var yearlyEvaluations = new YearlyStatDto(
            evaluations.Count.ToString("N0", Culture),
            "Evaluations logged across all academic years so far.",
            years.Select(y => (decimal)evaluations.Count(e => e.Date.Year == y)).ToList());

        var inboxDtos = inbox.Select(m => new InboxMessageDto(m.SenderName, m.Initials, m.Preview, m.Time.ToString("h:mm tt", Culture))).ToList();

        var activityDtos = activity.Select(a => new ActivityItemDto(a.Date.ToString("MMM d", Culture).ToUpperInvariant(), a.Title, a.Description)).ToList();

        var quote = instructor is null
            ? new QuoteDto(string.Empty, string.Empty, string.Empty, string.Empty)
            : new QuoteDto(instructor.Quote, instructor.Name, instructor.Title, instructor.Initials);

        var evaluationRows = latestEvaluations.Select(MapEvaluationRow).ToList();
        var submissionRows = latestSubmissions.Select(MapSubmissionRow).ToList();

        return new DashboardViewModel(
            new HeaderStatDto("Evaluations Done", evaluationsCompleted.ToString("N0", Culture)),
            new HeaderStatDto("Avg. Score", $"{averageScore.ToString("0.0", Culture)}%"),
            statCards,
            donut,
            scoreTrend,
            assessmentsOverview,
            inboxDtos,
            activityDtos,
            quote,
            yearlyEvaluations,
            evaluationRows,
            submissionRows);
    }

    private static EvaluationRowDto MapEvaluationRow(Evaluation e)
    {
        var (text, color) = e.Status switch
        {
            EvaluationStatus.Passed => ("Passed", "green"),
            EvaluationStatus.Pending => ("Pending", "amber"),
            _ => ("Failed", "red"),
        };
        return new EvaluationRowDto(
            e.Student?.Name ?? "Unknown",
            e.Student?.Initials ?? "?",
            text,
            color,
            $"{e.Score.ToString("0.0", Culture)}%",
            e.Date.ToString("M/d/yyyy", Culture));
    }

    private static SubmissionRowDto MapSubmissionRow(Submission s)
    {
        var (text, color) = s.Status switch
        {
            SubmissionStatus.Graded => ("Graded", "green"),
            SubmissionStatus.PendingReview => ("Pending Review", "amber"),
            _ => ("Rejected", "red"),
        };
        return new SubmissionRowDto(
            s.ReferenceCode,
            s.Student?.Name ?? "Unknown",
            s.Student?.Initials ?? "?",
            s.Subject,
            text,
            color,
            $"{s.Score.ToString("0.0", Culture)}%",
            s.Date.ToString("M/d/yyyy", Culture));
    }
}
