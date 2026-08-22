using Evaluate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Application.TeacherCourses.Queries.GetTeacherCoursesList;

public class GetTeacherCoursesListQueryHandler(IApplicationDbContext context, IIdentityService identityService) : IRequestHandler<GetTeacherCoursesListQuery, List<TeacherCourseDto>>
{
    public async Task<List<TeacherCourseDto>> Handle(GetTeacherCoursesListQuery request, CancellationToken cancellationToken)
    {
        var query = context.TeacherCourses.Include(tc => tc.Course).AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.TeacherUserId))
        {
            query = query.Where(tc => tc.TeacherUserId == request.TeacherUserId);
        }

        if (request.CourseId.HasValue)
        {
            query = query.Where(tc => tc.CourseId == request.CourseId);
        }

        var assignments = await query.ToListAsync(cancellationToken);

        var dtos = new List<TeacherCourseDto>(assignments.Count);
        foreach (var assignment in assignments)
        {
            var teacherName = await identityService.GetUserNameAsync(assignment.TeacherUserId, cancellationToken);
            dtos.Add(new TeacherCourseDto(
                assignment.Id,
                assignment.TeacherUserId,
                teacherName,
                assignment.CourseId,
                assignment.Course!.CourseName,
                assignment.AcademicYearId,
                assignment.ClassId,
                assignment.IsActive));
        }

        return dtos;
    }
}
