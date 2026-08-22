using Evaluate.Application.Dashboard;
using Evaluate.Application.Tests.Fakes;
using Evaluate.Domain.Entities.Dashboard;
using Evaluate.Domain.Enums.Dashboard;
using Xunit;

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
        Instructor? featuredInstructor = null)
    {
        return new DashboardService(
            new FakeStudentRepository(students),
            new FakeEvaluationRepository(evaluations),
            new FakeSubmissionRepository(submissions),
            new FakeActivityFeedRepository([]),
            new FakeInboxRepository([]),
            new FakeInstructorRepository(featuredInstructor));
    }

    [Fact]
    public async Task GetDashboardAsync_ComputesStudentCountAndPassRate()
    {
        var (students, evaluations, submissions) = BuildSampleData();
        var service = CreateService(students, evaluations, submissions);

        var result = await service.GetDashboardAsync();

        Assert.Equal("2", result.StatCards[0].Value);
        // 2 passed out of 4 evaluations = 50%
        Assert.Equal("50%", result.StatCards[2].Value);
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

    [Fact]
    public async Task GetDashboardAsync_WithNoFeaturedInstructor_ReturnsEmptyQuote()
    {
        var (students, evaluations, submissions) = BuildSampleData();
        var service = CreateService(students, evaluations, submissions);

        var result = await service.GetDashboardAsync();

        Assert.Equal(string.Empty, result.Quote.Text);
    }

    [Fact]
    public async Task GetDashboardAsync_WithFeaturedInstructor_MapsQuote()
    {
        var (students, evaluations, submissions) = BuildSampleData();
        var instructor = new Instructor { Name = "Dr. Marie Whitfield", Title = "Head of Assessment", Initials = "MW", Quote = "Test quote" };
        var service = CreateService(students, evaluations, submissions, instructor);

        var result = await service.GetDashboardAsync();

        Assert.Equal("Test quote", result.Quote.Text);
        Assert.Equal("Dr. Marie Whitfield", result.Quote.InstructorName);
    }
}
