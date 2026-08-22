using TeacherCourseEntity = Evaluate.Domain.Entities.Courses.TeacherCourse;

namespace Evaluate.Application.TeacherCourses;

public interface ITeacherCourseRepository
{
    Task<bool> ExistsAsync(string teacherUserId, int courseId, int academicYearId, int classId, CancellationToken cancellationToken = default);

    void Add(TeacherCourseEntity assignment);

    /// <summary>Each assignment's <see cref="Domain.Entities.Courses.Course"/> navigation is eagerly loaded.</summary>
    Task<List<TeacherCourseEntity>> GetAssignmentsAsync(string? teacherUserId, int? courseId, CancellationToken cancellationToken = default);

    Task<List<TeacherCourseEntity>> GetActiveForClassesAsync(List<int> classIds, int academicYearId, CancellationToken cancellationToken = default);

    Task<List<TeacherCourseEntity>> GetActiveForClassAsync(int classId, int academicYearId, CancellationToken cancellationToken = default);

    Task<List<TeacherCourseEntity>> GetActiveForTeacherAndClassAsync(string teacherUserId, int classId, int academicYearId, CancellationToken cancellationToken = default);
}
