namespace Evaluate.Application.Courses.Queries.GetCoursesList;

public record CourseDto(int Id, string CourseCode, string CourseName, string? Description, bool IsActive);
