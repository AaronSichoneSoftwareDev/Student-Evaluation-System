using Evaluate.Domain.Enums;

namespace Evaluate.Application.Students.Queries.GetActiveStudentsList;

public record ActiveStudentDto(
    int StudentId,
    string StudentNumber,
    string FullName,
    DateOnly DateOfBirth,
    Gender Gender,
    string? Email,
    int ClassId,
    string ClassName,
    DateOnly EnrollmentDate,
    bool ReportCardReady);
