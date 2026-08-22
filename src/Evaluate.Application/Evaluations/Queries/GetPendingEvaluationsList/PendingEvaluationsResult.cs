namespace Evaluate.Application.Evaluations.Queries.GetPendingEvaluationsList;

public record CourseOptionDto(int CourseId, string CourseName);

/// <summary>Distinguishes the "nothing to show" reasons (no current term set, teacher not
/// assigned to this class, or genuinely everyone's been evaluated) so the UI can explain
/// an empty table instead of just showing a blank one.</summary>
public record PendingEvaluationsResult(
    bool HasCurrentTerm,
    string? AcademicYearName,
    string? TermName,
    bool TeacherAssigned,
    List<CourseOptionDto> AvailableCourses,
    List<PendingEvaluationDto> Students);
