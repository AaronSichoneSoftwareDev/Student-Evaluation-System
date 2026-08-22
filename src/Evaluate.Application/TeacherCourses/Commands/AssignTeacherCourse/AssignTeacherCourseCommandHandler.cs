using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TeacherCourseEntity = Evaluate.Domain.Entities.Courses.TeacherCourse;

namespace Evaluate.Application.TeacherCourses.Commands.AssignTeacherCourse;

public class AssignTeacherCourseCommandHandler(IApplicationDbContext context, IIdentityService identityService) : IRequestHandler<AssignTeacherCourseCommand, Result<int>>
{
    public async Task<Result<int>> Handle(AssignTeacherCourseCommand request, CancellationToken cancellationToken)
    {
        var teacherName = await identityService.GetUserNameAsync(request.TeacherUserId, cancellationToken);
        if (teacherName is null)
        {
            throw new NotFoundException("User", request.TeacherUserId);
        }

        var courseExists = await context.Courses.AnyAsync(c => c.Id == request.CourseId, cancellationToken);
        if (!courseExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.Courses.Course), request.CourseId);
        }

        var alreadyAssigned = await context.TeacherCourses.AnyAsync(
            tc => tc.TeacherUserId == request.TeacherUserId && tc.CourseId == request.CourseId
                && tc.AcademicYearId == request.AcademicYearId && tc.ClassId == request.ClassId,
            cancellationToken);
        if (alreadyAssigned)
        {
            return Result<int>.Failure("This teacher is already assigned to that course for the given class and academic year.");
        }

        var assignment = TeacherCourseEntity.Assign(request.TeacherUserId, request.CourseId, request.AcademicYearId, request.ClassId);

        context.TeacherCourses.Add(assignment);
        await context.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(assignment.Id);
    }
}
