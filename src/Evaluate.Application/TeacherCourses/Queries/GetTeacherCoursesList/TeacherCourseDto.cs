namespace Evaluate.Application.TeacherCourses.Queries.GetTeacherCoursesList;

public record TeacherCourseDto(int Id, string TeacherUserId, string? TeacherName, int CourseId, string CourseName, int AcademicYearId, int ClassId, bool IsActive);
