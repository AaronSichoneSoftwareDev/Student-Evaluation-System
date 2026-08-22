using Evaluate.Domain.Common;

namespace Evaluate.Domain.Entities.Dashboard;

public class Instructor : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public string Quote { get; set; } = string.Empty;
}
