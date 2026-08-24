using Evaluate.Domain.Common;

namespace Evaluate.Domain.Entities.Academic;

public class AcademicYear : BaseAuditableEntity
{
    public string YearName { get; private set; } = string.Empty;
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public bool IsCurrent { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<Term> Terms { get; private set; } = new List<Term>();

    private AcademicYear()
    {
    }

    private AcademicYear(string yearName, DateOnly startDate, DateOnly endDate)
    {
        YearName = yearName;
        StartDate = startDate;
        EndDate = endDate;
    }

    public static AcademicYear Create(string yearName, DateOnly startDate, DateOnly endDate)
    {
        if (string.IsNullOrWhiteSpace(yearName))
        {
            throw new ArgumentException("Academic year name is required.", nameof(yearName));
        }

        if (endDate <= startDate)
        {
            throw new ArgumentException("End date must be after the start date.", nameof(endDate));
        }

        return new AcademicYear(yearName.Trim(), startDate, endDate);
    }

    public void Update(string yearName, DateOnly startDate, DateOnly endDate)
    {
        if (string.IsNullOrWhiteSpace(yearName))
        {
            throw new ArgumentException("Academic year name is required.", nameof(yearName));
        }

        if (endDate <= startDate)
        {
            throw new ArgumentException("End date must be after the start date.", nameof(endDate));
        }

        YearName = yearName.Trim();
        StartDate = startDate;
        EndDate = endDate;
    }

    public void MarkAsCurrent() => IsCurrent = true;

    public void UnmarkAsCurrent() => IsCurrent = false;

    public void Deactivate() => IsActive = false;
}
