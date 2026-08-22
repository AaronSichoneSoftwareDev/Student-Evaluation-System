using Evaluate.Domain.Enums;

namespace Evaluate.Application.Evaluations.Queries.GetEvaluationsList;

public record EvaluationDto(
    int Id,
    int StudentId,
    string StudentName,
    int CourseId,
    string CourseName,
    int TermId,
    int AcademicYearId,
    DateOnly EvaluationDate,
    EvaluationStatus Status,
    decimal? FinalPercentage,
    string? FinalGrade);
