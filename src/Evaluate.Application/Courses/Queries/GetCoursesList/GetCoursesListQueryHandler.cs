using MediatR;

namespace Evaluate.Application.Courses.Queries.GetCoursesList;

public class GetCoursesListQueryHandler(ICourseRepository courses) : IRequestHandler<GetCoursesListQuery, List<CourseDto>>
{
    public Task<List<CourseDto>> Handle(GetCoursesListQuery request, CancellationToken cancellationToken) =>
        courses.GetListAsync(cancellationToken);
}
