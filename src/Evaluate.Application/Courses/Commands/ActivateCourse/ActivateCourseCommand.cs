using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.Courses.Commands.ActivateCourse;

[RequirePermission(Permissions.Courses.Edit)]
public record ActivateCourseCommand(int Id) : IRequest<Result>;
