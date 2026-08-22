namespace Evaluate.Application.AcademicYears.Queries.GetAcademicYearsList;

public record AcademicYearDto(int Id, string YearName, DateOnly StartDate, DateOnly EndDate, bool IsCurrent, bool IsActive);
