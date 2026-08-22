using Evaluate.Domain.Enums;

namespace Evaluate.Application.Enrollments.Queries.GetEnrollmentsList;

public record EnrollmentDto(int Id, int StudentId, string StudentName, int AcademicYearId, int ClassId, string ClassName, DateOnly EnrollmentDate, EnrollmentStatus Status);
