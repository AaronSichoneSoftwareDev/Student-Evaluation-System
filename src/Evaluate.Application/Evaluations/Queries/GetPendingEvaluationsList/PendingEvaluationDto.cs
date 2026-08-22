namespace Evaluate.Application.Evaluations.Queries.GetPendingEvaluationsList;

/// <summary>A student who is missing a finalized evaluation in at least one of the
/// teacher's assigned subjects for this class — which subject(s) is decided when the
/// teacher clicks Evaluate, via <see cref="PendingEvaluationsResult.AvailableCourses"/>.</summary>
public record PendingEvaluationDto(int StudentId, string StudentNumber, string StudentName, int ClassId, string ClassName, int AcademicYearId, int TermId, string TermName);
