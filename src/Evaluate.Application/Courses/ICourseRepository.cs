using Evaluate.Application.Courses.Queries.GetCoursesList;
using CourseEntity = Evaluate.Domain.Entities.Courses.Course;

namespace Evaluate.Application.Courses;

public interface ICourseRepository
{
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByCodeAsync(string courseCode, CancellationToken cancellationToken = default);

    Task<CourseEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    void Add(CourseEntity course);

    Task<List<CourseDto>> GetListAsync(CancellationToken cancellationToken = default);

    Task<Dictionary<int, string>> GetNamesByIdsAsync(List<int> courseIds, CancellationToken cancellationToken = default);
}
