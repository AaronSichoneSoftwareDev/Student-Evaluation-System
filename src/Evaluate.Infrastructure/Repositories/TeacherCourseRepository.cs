using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.TeacherCourses;
using Microsoft.EntityFrameworkCore;
using TeacherCourseEntity = Evaluate.Domain.Entities.Courses.TeacherCourse;

namespace Evaluate.Infrastructure.Repositories;

public class TeacherCourseRepository(IApplicationDbContext context) : ITeacherCourseRepository
{
    public Task<bool> ExistsAsync(string teacherUserId, int courseId, int academicYearId, int classId, CancellationToken cancellationToken = default) =>
        context.TeacherCourses.AnyAsync(
            tc => tc.TeacherUserId == teacherUserId && tc.CourseId == courseId
                && tc.AcademicYearId == academicYearId && tc.ClassId == classId,
            cancellationToken);

    public void Add(TeacherCourseEntity assignment) => context.TeacherCourses.Add(assignment);

    public Task<List<TeacherCourseEntity>> GetAssignmentsAsync(string? teacherUserId, int? courseId, CancellationToken cancellationToken = default)
    {
        var query = context.TeacherCourses.Include(tc => tc.Course).AsQueryable();

        if (!string.IsNullOrWhiteSpace(teacherUserId))
        {
            query = query.Where(tc => tc.TeacherUserId == teacherUserId);
        }

        if (courseId.HasValue)
        {
            query = query.Where(tc => tc.CourseId == courseId);
        }

        return query.ToListAsync(cancellationToken);
    }

    public Task<List<TeacherCourseEntity>> GetActiveForClassesAsync(List<int> classIds, int academicYearId, CancellationToken cancellationToken = default) =>
        context.TeacherCourses
            .Include(tc => tc.Course)
            .Where(tc => classIds.Contains(tc.ClassId) && tc.AcademicYearId == academicYearId && tc.IsActive)
            .ToListAsync(cancellationToken);

    public Task<List<TeacherCourseEntity>> GetActiveForClassAsync(int classId, int academicYearId, CancellationToken cancellationToken = default) =>
        context.TeacherCourses
            .Include(tc => tc.Course)
            .Where(tc => tc.ClassId == classId && tc.AcademicYearId == academicYearId && tc.IsActive)
            .ToListAsync(cancellationToken);

    public Task<List<TeacherCourseEntity>> GetActiveForTeacherAndClassAsync(string teacherUserId, int classId, int academicYearId, CancellationToken cancellationToken = default) =>
        context.TeacherCourses
            .Include(tc => tc.Course)
            .Where(
                tc => tc.TeacherUserId == teacherUserId && tc.ClassId == classId
                    && tc.AcademicYearId == academicYearId && tc.IsActive)
            .ToListAsync(cancellationToken);
}
