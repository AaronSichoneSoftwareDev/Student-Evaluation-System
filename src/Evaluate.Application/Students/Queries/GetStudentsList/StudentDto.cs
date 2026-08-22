using Evaluate.Domain.Enums;

namespace Evaluate.Application.Students.Queries.GetStudentsList;

public record StudentDto(int Id, string StudentNumber, string FullName, DateOnly DateOfBirth, Gender Gender, string? Email, bool IsActive);
