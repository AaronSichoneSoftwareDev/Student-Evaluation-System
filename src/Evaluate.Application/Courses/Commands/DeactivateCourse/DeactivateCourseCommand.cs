using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.Courses.Commands.DeactivateCourse;

[RequirePermission(Permissions.Courses.Edit)]
public record DeactivateCourseCommand(int Id) : IRequest<Result>;
