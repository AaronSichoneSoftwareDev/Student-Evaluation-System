using Evaluate.Application.Common.Interfaces;
using Evaluate.Domain.Entities.Dashboard;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Infrastructure.Persistence.Repositories;

public class ActivityFeedRepository(EvaluateDbContext context) : IActivityFeedRepository
{
    public async Task<IReadOnlyList<ActivityFeedItem>> GetRecentAsync(int count, CancellationToken cancellationToken = default)
        => await context.ActivityFeedItems.AsNoTracking()
            .OrderByDescending(a => a.Date).ThenByDescending(a => a.Id)
            .Take(count).ToListAsync(cancellationToken);
}
