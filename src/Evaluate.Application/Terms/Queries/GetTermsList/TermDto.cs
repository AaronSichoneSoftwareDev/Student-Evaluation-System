namespace Evaluate.Application.Terms.Queries.GetTermsList;

public record TermDto(int Id, int AcademicYearId, string TermName, int TermNumber, DateOnly StartDate, DateOnly EndDate, bool IsActive, bool IsCurrent);
