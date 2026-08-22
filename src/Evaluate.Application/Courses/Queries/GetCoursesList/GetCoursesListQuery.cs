using MediatR;

namespace Evaluate.Application.Courses.Queries.GetCoursesList;

public record GetCoursesListQuery : IRequest<List<CourseDto>>;
