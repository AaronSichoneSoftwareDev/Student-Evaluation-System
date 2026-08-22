using Evaluate.Domain.Enums;

namespace Evaluate.Application.Students.Queries.GetStudentById;

public record StudentDetailDto(
    int Id,
    string StudentNumber,
    string FirstName,
    string? MiddleName,
    string LastName,
    DateOnly DateOfBirth,
    Gender Gender,
    string? Email,
    string? PhoneNumber,
    string? Address,
    bool IsActive,
    int? ClassId,
    string? ClassName);
