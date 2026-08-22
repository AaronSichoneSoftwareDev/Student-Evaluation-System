using Evaluate.Domain.Common;
using Evaluate.Domain.Enums.Dashboard;

namespace Evaluate.Domain.Entities.Dashboard;

public class Submission : BaseEntity
{
    public string ReferenceCode { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public string Subject { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public SubmissionStatus Status { get; set; }
    public DateOnly Date { get; set; }
}
