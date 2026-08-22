using Evaluate.Domain.Common;

namespace Evaluate.Domain.Entities.Dashboard;

public class InboxMessage : BaseEntity
{
    public string SenderName { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public string Preview { get; set; } = string.Empty;
    public TimeOnly Time { get; set; }
}
