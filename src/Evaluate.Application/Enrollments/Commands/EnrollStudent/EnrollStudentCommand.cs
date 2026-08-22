using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.Enrollments.Commands.EnrollStudent;

[RequirePermission(Permissions.Enrollments.Create)]
public record EnrollStudentCommand(int StudentId, int AcademicYearId, int ClassId, DateOnly EnrollmentDate) : IRequest<Result<int>>;
