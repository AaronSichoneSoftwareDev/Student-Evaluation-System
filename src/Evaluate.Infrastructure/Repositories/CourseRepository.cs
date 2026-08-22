using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Courses;
using Evaluate.Application.Courses.Queries.GetCoursesList;
using Microsoft.EntityFrameworkCore;
using CourseEntity = Evaluate.Domain.Entities.Courses.Course;

namespace Evaluate.Infrastructure.Repositories;

public class CourseRepository(IApplicationDbContext context) : ICourseRepository
{
    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        context.Courses.AnyAsync(c => c.Id == id, cancellationToken);

    public Task<bool> ExistsByCodeAsync(string courseCode, CancellationToken cancellationToken = default) =>
        context.Courses.AnyAsync(c => c.CourseCode == courseCode, cancellationToken);

    public Task<CourseEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Courses.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public void Add(CourseEntity course) => context.Courses.Add(course);

    public Task<List<CourseDto>> GetListAsync(CancellationToken cancellationToken = default) =>
        context.Courses
            .OrderBy(c => c.CourseName)
            .Select(c => new CourseDto(c.Id, c.CourseCode, c.CourseName, c.Description, c.IsActive))
            .ToListAsync(cancellationToken);

    public Task<Dictionary<int, string>> GetNamesByIdsAsync(List<int> courseIds, CancellationToken cancellationToken = default) =>
        context.Courses
            .Where(c => courseIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.CourseName, cancellationToken);
}
