using MediatR;

namespace Evaluate.Application.TeacherCourses.Queries.GetTeacherCoursesList;

public record GetTeacherCoursesListQuery(string? TeacherUserId = null, int? CourseId = null) : IRequest<List<TeacherCourseDto>>;
