using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using MediatR;

namespace Evaluate.Application.TeacherCourses.Commands.AssignTeacherCourse;

[RequirePermission(Permissions.TeacherCourses.Create)]
public record AssignTeacherCourseCommand(string TeacherUserId, int CourseId, int AcademicYearId, int ClassId) : IRequest<Result<int>>;
