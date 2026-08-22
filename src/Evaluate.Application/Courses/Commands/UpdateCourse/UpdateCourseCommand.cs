using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.Courses.Commands.UpdateCourse;

[RequirePermission(Permissions.Courses.Edit)]
public record UpdateCourseCommand(int Id, string CourseName, string? Description = null) : IRequest<Result>;
