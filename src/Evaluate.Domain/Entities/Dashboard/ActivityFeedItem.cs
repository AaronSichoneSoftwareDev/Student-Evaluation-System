using Evaluate.Domain.Common;

namespace Evaluate.Domain.Entities.Dashboard;

public class ActivityFeedItem : BaseEntity
{
    public DateOnly Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
