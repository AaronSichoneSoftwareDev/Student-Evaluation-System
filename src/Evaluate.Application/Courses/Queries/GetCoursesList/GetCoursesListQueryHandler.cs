using Evaluate.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Application.Courses.Queries.GetCoursesList;

public class GetCoursesListQueryHandler(IApplicationDbContext context) : IRequestHandler<GetCoursesListQuery, List<CourseDto>>
{
    public async Task<List<CourseDto>> Handle(GetCoursesListQuery request, CancellationToken cancellationToken) =>
        await context.Courses
            .OrderBy(c => c.CourseName)
            .Select(c => new CourseDto(c.Id, c.CourseCode, c.CourseName, c.Description, c.IsActive))
            .ToListAsync(cancellationToken);
}
