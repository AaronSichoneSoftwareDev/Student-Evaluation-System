using Evaluate.Application.Dashboard;
using Evaluate.Application.Tests.Common;
using Evaluate.Application.Tests.Fakes;
using Evaluate.Domain.Entities.Dashboard;
using Evaluate.Domain.Enums.Dashboard;
using Xunit;
using AcademicYearEntity = Evaluate.Domain.Entities.Academic.AcademicYear;
using SchoolClassEntity = Evaluate.Domain.Entities.Academic.SchoolClass;
using StudentEntity = Evaluate.Domain.Entities.People.Student;
using StudentEnrollmentEntity = Evaluate.Domain.Entities.People.StudentEnrollment;

namespace Evaluate.Application.Tests.Dashboard;

public class DashboardServiceTests
{
    private static (List<Student> Students, List<Evaluation> Evaluations, List<Submission> Submissions) BuildSampleData()
    {
        var students = new List<Student>
        {
            new() { Id = 1, Name = "Ava Thompson", Initials = "AT" },
            new() { Id = 2, Name = "Liam Carter", Initials = "LC" },
        };

        var evaluations = new List<Evaluation>
        {
            new() { Id = 1, StudentId = 1, Student = students[0], Subject = "Mathematics", Score = 90, Status = EvaluationStatus.Passed, Date = new DateOnly(2024, 3, 1) },
            new() { Id = 2, StudentId = 1, Student = students[0], Subject = "Science", Score = 85, Status = EvaluationStatus.Passed, Date = new DateOnly(2025, 3, 1) },
            new() { Id = 3, StudentId = 2, Student = students[1], Subject = "English", Score = 40, Status = EvaluationStatus.Failed, Date = new DateOnly(2024, 5, 1) },
            new() { Id = 4, StudentId = 2, Student = students[1], Subject = "History", Score = 60, Status = EvaluationStatus.Pending, Date = new DateOnly(2025, 5, 1) },
        };

        var submissions = new List<Submission>
        {
            new() { Id = 1, StudentId = 1, Student = students[0], Subject = "Mathematics", Score = 88, Status = SubmissionStatus.Graded, Date = new DateOnly(2024, 3, 5) },
            new() { Id = 2, StudentId = 2, Student = students[1], Subject = "English", Score = 55, Status = SubmissionStatus.PendingReview, Date = new DateOnly(2025, 5, 5) },
        };

        return (students, evaluations, submissions);
    }

    private static DashboardService CreateService(
        List<Student> students,
        List<Evaluation> evaluations,
        List<Submission> submissions,
        TestDbContext? context = null)
    {
        context ??= TestDbContext.Create();
        return new DashboardService(
            new FakeStudentRepository(students),
            new FakeEvaluationRepository(evaluations),
            new FakeSubmissionRepository(submissions),
            new FakeActivityFeedRepository([]),
            RepositoryFactory.AcademicYears(context),
            RepositoryFactory.Enrollments(context));
    }

    [Fact]
    public async Task GetDashboardAsync_ComputesPupilsEvaluatedRateAndPassRate()
    {
        var (students, evaluations, submissions) = BuildSampleData();
        var service = CreateService(students, evaluations, submissions);

        var result = await service.GetDashboardAsync();

        // Both seeded students have at least one evaluation = 100%
        Assert.Equal("100%", result.StatCards[0].Value);
        Assert.False(result.StatCards[0].IsAlert);
        // 2 passed out of 4 evaluations = 50%
        Assert.Equal("50%", result.StatCards[2].Value);
    }

    [Fact]
    public async Task GetDashboardAsync_WithLowPupilsEvaluatedRate_FlagsAlert()
    {
        var students = new List<Student>
        {
            new() { Id = 1, Name = "Ava Thompson", Initials = "AT" },
            new() { Id = 2, Name = "Liam Carter", Initials = "LC" },
            new() { Id = 3, Name = "Noah Bennett", Initials = "NB" },
        };
        var evaluations = new List<Evaluation>
        {
            new() { Id = 1, StudentId = 1, Student = students[0], Subject = "Mathematics", Score = 90, Status = EvaluationStatus.Passed, Date = new DateOnly(2025, 3, 1) },
        };
        var service = CreateService(students, evaluations, []);

        var result = await service.GetDashboardAsync();

        // 1 of 3 students evaluated = 33%, below the 70% threshold
        Assert.Equal("33%", result.StatCards[0].Value);
        Assert.True(result.StatCards[0].IsAlert);
    }

    [Fact]
    public async Task GetDashboardAsync_WithNoCurrentAcademicYear_ReportsZeroEligibleStudents()
    {
        var (students, evaluations, submissions) = BuildSampleData();
        var service = CreateService(students, evaluations, submissions);

        var result = await service.GetDashboardAsync();

        Assert.Equal("0", result.StatCards[3].Value);
    }

    [Fact]
    public async Task GetDashboardAsync_CountsActivelyEnrolledPupilsForCurrentAcademicYear()
    {
        var (students, evaluations, submissions) = BuildSampleData();
        var context = TestDbContext.Create();

        var year = AcademicYearEntity.Create("2026", new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));
        year.MarkAsCurrent();
        context.AcademicYears.Add(year);
        await context.SaveChangesAsync(CancellationToken.None);

        var schoolClass = SchoolClassEntity.Create("Grade 7A", "Grade 7");
        context.Classes.Add(schoolClass);
        var studentA = StudentEntity.Create("STU001", "Peter", "Banda", new DateOnly(2013, 3, 14), Evaluate.Domain.Enums.Gender.Male);
        var studentB = StudentEntity.Create("STU002", "Mary", "Phiri", new DateOnly(2013, 7, 2), Evaluate.Domain.Enums.Gender.Female);
        context.Students.AddRange(studentA, studentB);
        await context.SaveChangesAsync(CancellationToken.None);

        var enrollmentA = StudentEnrollmentEntity.Enroll(studentA.Id, year.Id, schoolClass.Id, new DateOnly(2026, 1, 12));
        var enrollmentB = StudentEnrollmentEntity.Enroll(studentB.Id, year.Id, schoolClass.Id, new DateOnly(2026, 1, 12));
        enrollmentB.Withdraw();
        context.StudentEnrollments.AddRange(enrollmentA, enrollmentB);
        await context.SaveChangesAsync(CancellationToken.None);

        var service = CreateService(students, evaluations, submissions, context);

        var result = await service.GetDashboardAsync();

        // Only the active enrollment counts toward "eligible for evaluation".
        Assert.Equal("1", result.StatCards[3].Value);
    }

    [Fact]
    public async Task GetDashboardAsync_OutcomesTrend_ComputesPerYearPassFailPendingRates()
    {
        var (students, evaluations, submissions) = BuildSampleData();
        var service = CreateService(students, evaluations, submissions);

        var result = await service.GetDashboardAsync();

        Assert.Equal(["2024", "2025"], result.OutcomesTrend.Categories);
        var passSeries = result.OutcomesTrend.Series.Single(s => s.Name == "Pass Rate");
        var failSeries = result.OutcomesTrend.Series.Single(s => s.Name == "Fail Rate");
        var pendingSeries = result.OutcomesTrend.Series.Single(s => s.Name == "Pending Rate");
        // 2024: 1 passed + 1 failed = 50/50. 2025: 1 passed + 1 pending = 50/50.
        Assert.Equal([50m, 50m], passSeries.Values);
        Assert.Equal([50m, 0m], failSeries.Values);
        Assert.Equal([0m, 50m], pendingSeries.Values);
        // Latest year (2025) pass rate is 50%, fail rate is 0%.
        Assert.Equal("50%", result.OutcomesTrend.Total1Value);
        Assert.Equal("0%", result.OutcomesTrend.Total2Value);
    }

    [Fact]
    public async Task GetDashboardAsync_BacklogAndRiskOverview_SeparatesPendingFromRejected()
    {
        var (students, evaluations, _) = BuildSampleData();
        var submissions = new List<Submission>
        {
            new() { Id = 1, StudentId = 1, Student = students[0], Subject = "Mathematics", Score = 88, Status = SubmissionStatus.Graded, Date = new DateOnly(2025, 3, 5) },
            new() { Id = 2, StudentId = 2, Student = students[1], Subject = "English", Score = 55, Status = SubmissionStatus.PendingReview, Date = new DateOnly(2025, 5, 5) },
            new() { Id = 3, StudentId = 2, Student = students[1], Subject = "History", Score = 30, Status = SubmissionStatus.Rejected, Date = new DateOnly(2025, 6, 1) },
        };
        var service = CreateService(students, evaluations, submissions);

        var result = await service.GetDashboardAsync();

        Assert.Equal("1", result.BacklogAndRiskOverview.Total1Value); // Pending Review
        Assert.Equal("1", result.BacklogAndRiskOverview.Total2Value); // Rejected
        var pendingSeries = result.BacklogAndRiskOverview.Series.Single(s => s.Name == "Pending Review");
        var rejectedSeries = result.BacklogAndRiskOverview.Series.Single(s => s.Name == "Rejected");
        Assert.Equal([1m], pendingSeries.Values);
        Assert.Equal([1m], rejectedSeries.Values);
    }

    [Fact]
    public async Task GetDashboardAsync_FlagsFailedAndOverdueEvaluations()
    {
        var students = new List<Student>
        {
            new() { Id = 1, Name = "Ava Thompson", Initials = "AT" },
            new() { Id = 2, Name = "Liam Carter", Initials = "LC" },
            new() { Id = 3, Name = "Noah Bennett", Initials = "NB" },
        };
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var evaluations = new List<Evaluation>
        {
            new() { Id = 1, StudentId = 1, Student = students[0], Subject = "Mathematics", Score = 40, Status = EvaluationStatus.Failed, Date = today.AddDays(-1) },
            new() { Id = 2, StudentId = 2, Student = students[1], Subject = "Science", Score = 60, Status = EvaluationStatus.Pending, Date = today.AddDays(-20) },
            new() { Id = 3, StudentId = 3, Student = students[2], Subject = "English", Score = 90, Status = EvaluationStatus.Passed, Date = today.AddDays(-2) },
        };
        var service = CreateService(students, evaluations, []);

        var result = await service.GetDashboardAsync();

        var failedRow = result.LatestEvaluations.Single(r => r.StudentName == "Ava Thompson");
        Assert.Equal("At Risk", failedRow.FlagText);
        Assert.Equal("red", failedRow.FlagColor);

        // Pending for 20 days > the 14-day overdue threshold.
        var overdueRow = result.LatestEvaluations.Single(r => r.StudentName == "Liam Carter");
        Assert.Equal("Overdue", overdueRow.FlagText);
        Assert.Equal("amber", overdueRow.FlagColor);

        var healthyRow = result.LatestEvaluations.Single(r => r.StudentName == "Noah Bennett");
        Assert.Null(healthyRow.FlagText);
    }

    [Fact]
    public async Task GetDashboardAsync_ActivityFeed_SurfacesRiskAndOverdueEvents()
    {
        var students = new List<Student>
        {
            new() { Id = 1, Name = "Ava Thompson", Initials = "AT" },
            new() { Id = 2, Name = "Liam Carter", Initials = "LC" },
        };
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var evaluations = new List<Evaluation>
        {
            new() { Id = 1, StudentId = 1, Student = students[0], Subject = "Mathematics", Score = 40, Status = EvaluationStatus.Failed, Date = today.AddDays(-1) },
            new() { Id = 2, StudentId = 2, Student = students[1], Subject = "Science", Score = 60, Status = EvaluationStatus.Pending, Date = today.AddDays(-30) },
        };
        var submissions = new List<Submission>
        {
            new() { Id = 1, StudentId = 1, Student = students[0], Subject = "Mathematics", Score = 30, Status = SubmissionStatus.Rejected, Date = today.AddDays(-1) },
        };
        var service = CreateService(students, evaluations, submissions);

        var result = await service.GetDashboardAsync();

        Assert.Contains(result.ActivityFeed, a => a.Category == "Exposed Risk" && a.Title.Contains("Evaluation failed"));
        Assert.Contains(result.ActivityFeed, a => a.Category == "Exposed Risk" && a.Title.Contains("Submission rejected"));
        Assert.Contains(result.ActivityFeed, a => a.Category == "Delayed" && a.Title.Contains("overdue"));
    }

    [Fact]
    public async Task GetDashboardAsync_DonutSegmentsSumToTotalEvaluations()
    {
        var (students, evaluations, submissions) = BuildSampleData();
        var service = CreateService(students, evaluations, submissions);

        var result = await service.GetDashboardAsync();

        var total = result.PerformanceDonut.Segments.Sum(s => s.Value);
        Assert.Equal(evaluations.Count, total);
    }

    [Fact]
    public async Task GetDashboardAsync_LatestEvaluations_AreOrderedByDateDescending()
    {
        var (students, evaluations, submissions) = BuildSampleData();
        var service = CreateService(students, evaluations, submissions);

        var result = await service.GetDashboardAsync();

        Assert.Equal(evaluations.Count, result.LatestEvaluations.Count);
        Assert.Equal("Liam Carter", result.LatestEvaluations[0].StudentName);
    }
}
