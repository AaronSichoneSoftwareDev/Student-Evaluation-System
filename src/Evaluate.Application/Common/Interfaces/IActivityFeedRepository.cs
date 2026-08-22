using Evaluate.Domain.Entities.Dashboard;

namespace Evaluate.Application.Common.Interfaces;

public interface IActivityFeedRepository
{
    Task<IReadOnlyList<ActivityFeedItem>> GetRecentAsync(int count, CancellationToken cancellationToken = default);
}
