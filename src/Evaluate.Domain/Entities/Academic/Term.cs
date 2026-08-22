using Evaluate.Domain.Common;

namespace Evaluate.Domain.Entities.Academic;

public class Term : BaseAuditableEntity
{
    public int AcademicYearId { get; private set; }
    public AcademicYear? AcademicYear { get; private set; }
    public string TermName { get; private set; } = string.Empty;
    public int TermNumber { get; private set; }
    public DateOnly StartDate { get; private set; }
    public DateOnly EndDate { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsCurrent { get; private set; }

    private Term()
    {
    }

    private Term(int academicYearId, string termName, int termNumber, DateOnly startDate, DateOnly endDate)
    {
        AcademicYearId = academicYearId;
        TermName = termName;
        TermNumber = termNumber;
        StartDate = startDate;
        EndDate = endDate;
    }

    public static Term Create(int academicYearId, string termName, int termNumber, DateOnly startDate, DateOnly endDate)
    {
        if (string.IsNullOrWhiteSpace(termName))
        {
            throw new ArgumentException("Term name is required.", nameof(termName));
        }

        if (termNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(termNumber), "Term number must be positive.");
        }

        if (endDate <= startDate)
        {
            throw new ArgumentException("End date must be after the start date.", nameof(endDate));
        }

        return new Term(academicYearId, termName.Trim(), termNumber, startDate, endDate);
    }

    public void Deactivate() => IsActive = false;

    public void MarkAsCurrent() => IsCurrent = true;

    public void UnmarkAsCurrent() => IsCurrent = false;
}
