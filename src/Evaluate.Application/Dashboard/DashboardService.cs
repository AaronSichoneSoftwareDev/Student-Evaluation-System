using System.Globalization;
using Evaluate.Application.AcademicYears;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Enrollments;
using Evaluate.Domain.Entities.Dashboard;
using Evaluate.Domain.Enums.Dashboard;

namespace Evaluate.Application.Dashboard;

public class DashboardService(
    IStudentRepository studentRepository,
    IEvaluationRepository evaluationRepository,
    ISubmissionRepository submissionRepository,
    IActivityFeedRepository activityFeedRepository,
    IAcademicYearRepository academicYearRepository,
    IEnrollmentRepository enrollmentRepository) : IDashboardService
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;
    private const decimal PupilsEvaluatedAlertThreshold = 70m;
    private const int OverdueEvaluationDays = 14;

    // Chart series colors — drawn only from the brand palette (gold/periwinkle/amber), no green
    // or red, matching the same substitutions made for --green/--red in dashboard.css.
    private const string ChartGoodColor = "#F7B700";
    private const string ChartCautionColor = "#f2a93b";
    private const string ChartBadColor = "#3F4C73";

    public async Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var students = await studentRepository.GetAllAsync(cancellationToken);
        var evaluations = await evaluationRepository.GetAllAsync(cancellationToken);
        var submissions = await submissionRepository.GetAllAsync(cancellationToken);
        var latestEvaluations = await evaluationRepository.GetLatestAsync(7, cancellationToken);
        var activity = await activityFeedRepository.GetRecentAsync(5, cancellationToken);

        var averageScore = evaluations.Count > 0 ? evaluations.Average(e => e.Score) : 0m;
        var passedCount = evaluations.Count(e => e.Status == EvaluationStatus.Passed);
        var pendingCount = evaluations.Count(e => e.Status == EvaluationStatus.Pending);
        var failedCount = evaluations.Count(e => e.Status == EvaluationStatus.Failed);
        var passRate = evaluations.Count > 0 ? (decimal)passedCount / evaluations.Count * 100 : 0m;
        var evaluationsCompleted = evaluations.Count(e => e.Status != EvaluationStatus.Pending);

        var evaluatedPupilCount = evaluations.Select(e => e.StudentId).Distinct().Count();
        var pupilsEvaluatedRate = students.Count > 0 ? (decimal)evaluatedPupilCount / students.Count * 100 : 0m;

        var currentYear = await academicYearRepository.GetCurrentAsync(cancellationToken);
        var eligibleStudentCount = currentYear is null
            ? 0
            : await enrollmentRepository.CountActiveByAcademicYearAsync(currentYear.Id, cancellationToken);

        var statCards = new List<StatCardDto>
        {
            new("Pupils Evaluated", $"{pupilsEvaluatedRate.ToString("0", Culture)}%", "bi-people-fill", "+12%", true, pupilsEvaluatedRate < PupilsEvaluatedAlertThreshold),
            new("Average Score", $"{averageScore.ToString("0.0", Culture)}%", "bi-graph-up-arrow", "+4%", true),
            new("Pass Rate", $"{passRate.ToString("0", Culture)}%", "bi-award-fill", "+8%", true),
            new("Eligible Students", eligibleStudentCount.ToString("N0", Culture), "bi-mortarboard-fill", "+5%", true),
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
                new("Passed", passedCount, ChartGoodColor),
                new("Pending", pendingCount, ChartCautionColor),
                new("Failed", failedCount, ChartBadColor),
            });

        var years = evaluations.Select(e => e.Date.Year).Distinct().OrderBy(y => y).ToList();
        if (years.Count == 0) years = [DateTime.UtcNow.Year];

        var evaluationsByYear = years.ToDictionary(y => y, y => evaluations.Where(e => e.Date.Year == y).ToList());
        var passRateByYear = years.Select(y => RateOf(evaluationsByYear[y], EvaluationStatus.Passed)).ToList();
        var failRateByYear = years.Select(y => RateOf(evaluationsByYear[y], EvaluationStatus.Failed)).ToList();
        var pendingRateByYear = years.Select(y => RateOf(evaluationsByYear[y], EvaluationStatus.Pending)).ToList();

        // Pass/fail/pending rate over time — unlike raw score highs/lows, this tells a manager
        // whether outcomes are actually improving and whether the pending backlog is shrinking.
        var outcomesTrend = new CategoryChartDto(
            "Evaluation Outcomes Trend",
            "Latest Pass Rate",
            $"{passRateByYear[^1].ToString("0", Culture)}%",
            "Latest Fail Rate",
            $"{failRateByYear[^1].ToString("0", Culture)}%",
            years.Select(y => y.ToString(Culture)).ToList(),
            new List<ChartSeriesDto>
            {
                new("Pass Rate", passRateByYear, ChartGoodColor),
                new("Fail Rate", failRateByYear, ChartBadColor),
                new("Pending Rate", pendingRateByYear, ChartCautionColor),
            });

        var subYears = submissions.Select(s => s.Date.Year).Distinct().OrderBy(y => y).ToList();
        if (subYears.Count == 0) subYears = [DateTime.UtcNow.Year];
        var gradedByYear = subYears.Select(y => (decimal)submissions.Count(s => s.Date.Year == y && s.Status == SubmissionStatus.Graded)).ToList();
        var pendingReviewByYear = subYears.Select(y => (decimal)submissions.Count(s => s.Date.Year == y && s.Status == SubmissionStatus.PendingReview)).ToList();
        var rejectedByYear = subYears.Select(y => (decimal)submissions.Count(s => s.Date.Year == y && s.Status == SubmissionStatus.Rejected)).ToList();

        var totalPendingReview = submissions.Count(s => s.Status == SubmissionStatus.PendingReview);
        var totalRejected = submissions.Count(s => s.Status == SubmissionStatus.Rejected);

        // A growing Pending Review bar is wasted time (work sitting in a backlog); a growing
        // Rejected bar is exposed risk (quality/compliance issues that can turn into disputes).
        // Splitting them out (instead of lumping everything non-Graded together) is what lets a
        // manager tell the two apart and act on whichever is actually the problem.
        var backlogAndRiskOverview = new CategoryChartDto(
            "Submission Backlog & Risk",
            "Pending Review",
            totalPendingReview.ToString("N0", Culture),
            "Rejected",
            totalRejected.ToString("N0", Culture),
            subYears.Select(y => y.ToString(Culture)).ToList(),
            new List<ChartSeriesDto>
            {
                new("Graded", gradedByYear, ChartGoodColor),
                new("Pending Review", pendingReviewByYear, ChartCautionColor),
                new("Rejected", rejectedByYear, ChartBadColor),
            });

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // Recent failed evaluations and rejected submissions are Exposed Risk (they can become
        // parent disputes or compliance findings); evaluations still Pending well past the
        // expected turnaround are Delayed — and until they're resolved, anyone reading the
        // record is working with Missing Data. All three are timestamped from real records (the
        // overdue ones as "today", since that's when the delay is actually being observed) and
        // merged into the same feed so the most decision-relevant items surface first.
        var riskEvents = new List<ActivityCandidate>();
        riskEvents.AddRange(evaluations
            .Where(e => e.Status == EvaluationStatus.Failed)
            .OrderByDescending(e => e.Date)
            .Take(2)
            .Select(e => new ActivityCandidate(
                e.Date,
                $"Evaluation failed — {e.Student?.Name ?? "Unknown"}",
                $"{e.Subject}, scored {e.Score.ToString("0.0", Culture)}%. May need parent follow-up.",
                "Exposed Risk", "red")));
        riskEvents.AddRange(submissions
            .Where(s => s.Status == SubmissionStatus.Rejected)
            .OrderByDescending(s => s.Date)
            .Take(2)
            .Select(s => new ActivityCandidate(
                s.Date,
                $"Submission rejected — {s.Student?.Name ?? "Unknown"}",
                $"{s.Subject}, ref {s.ReferenceCode}. Flagged during review.",
                "Exposed Risk", "red")));
        riskEvents.AddRange(evaluations
            .Where(e => e.Status == EvaluationStatus.Pending && today.DayNumber - e.Date.DayNumber > OverdueEvaluationDays)
            .OrderBy(e => e.Date)
            .Take(2)
            .Select(e => new ActivityCandidate(
                today,
                $"Evaluation overdue — {e.Student?.Name ?? "Unknown"}",
                $"{e.Subject} still pending after {today.DayNumber - e.Date.DayNumber} days — delays the result and leaves the record incomplete.",
                "Delayed", "amber")));

        var activityDtos = activity
            .Select(a => new ActivityCandidate(a.Date, a.Title, a.Description, null, null))
            .Concat(riskEvents)
            .OrderByDescending(a => a.Date)
            .Take(5)
            .Select(a => new ActivityItemDto(a.Date.ToString("MMM d", Culture).ToUpperInvariant(), a.Title, a.Description, a.Category, a.CategoryColor))
            .ToList();

        var evaluationRows = latestEvaluations.Select(e => MapEvaluationRow(e, today)).ToList();

        return new DashboardViewModel(
            new HeaderStatDto("Evaluations Done", evaluationsCompleted.ToString("N0", Culture)),
            new HeaderStatDto("Avg. Score", $"{averageScore.ToString("0.0", Culture)}%"),
            statCards,
            donut,
            outcomesTrend,
            backlogAndRiskOverview,
            activityDtos,
            evaluationRows);
    }

    private static decimal RateOf(List<Evaluation> evaluations, EvaluationStatus status) =>
        evaluations.Count > 0 ? (decimal)evaluations.Count(e => e.Status == status) / evaluations.Count * 100 : 0m;

    private static EvaluationRowDto MapEvaluationRow(Evaluation e, DateOnly today)
    {
        var (text, color) = e.Status switch
        {
            EvaluationStatus.Passed => ("Passed", "green"),
            EvaluationStatus.Pending => ("Pending", "amber"),
            _ => ("Failed", "red"),
        };

        (string? flagText, string? flagColor) = e.Status switch
        {
            EvaluationStatus.Failed => ("At Risk", "red"),
            EvaluationStatus.Pending when today.DayNumber - e.Date.DayNumber > OverdueEvaluationDays => ("Overdue", "amber"),
            _ => (null, null),
        };

        return new EvaluationRowDto(
            e.Student?.Name ?? "Unknown",
            e.Student?.Initials ?? "?",
            text,
            color,
            $"{e.Score.ToString("0.0", Culture)}%",
            e.Date.ToString("M/d/yyyy", Culture),
            flagText,
            flagColor);
    }

    private sealed record ActivityCandidate(DateOnly Date, string Title, string Description, string? Category, string? CategoryColor);
}
