using Evaluate.Application.Common.Interfaces;
using Evaluate.Domain.Entities.Dashboard;

namespace Evaluate.Application.Tests.Fakes;

public class FakeStudentRepository(IReadOnlyList<Student> students) : IStudentRepository
{
    public Task<IReadOnlyList<Student>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(students);
}

public class FakeEvaluationRepository(IReadOnlyList<Evaluation> evaluations) : IEvaluationRepository
{
    public Task<IReadOnlyList<Evaluation>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(evaluations);

    public Task<IReadOnlyList<Evaluation>> GetLatestAsync(int count, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Evaluation>>(evaluations.OrderByDescending(e => e.Date).Take(count).ToList());
}

public class FakeSubmissionRepository(IReadOnlyList<Submission> submissions) : ISubmissionRepository
{
    public Task<IReadOnlyList<Submission>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(submissions);
}

public class FakeActivityFeedRepository(IReadOnlyList<ActivityFeedItem> items) : IActivityFeedRepository
{
    public Task<IReadOnlyList<ActivityFeedItem>> GetRecentAsync(int count, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ActivityFeedItem>>(items.OrderByDescending(i => i.Date).Take(count).ToList());
}
