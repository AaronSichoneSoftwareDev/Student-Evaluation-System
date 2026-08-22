using Evaluate.Domain.Common;

namespace Evaluate.Domain.Entities.Dashboard;

public class Student : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Initials { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
