using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.Courses.Commands.CreateCourse;

[RequirePermission(Permissions.Courses.Create)]
public record CreateCourseCommand(string CourseCode, string CourseName, string? Description = null) : IRequest<Result<int>>;
