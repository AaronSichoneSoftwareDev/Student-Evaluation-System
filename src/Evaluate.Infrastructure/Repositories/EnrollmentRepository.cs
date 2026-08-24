using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Enrollments;
using Evaluate.Application.Enrollments.Queries.GetEnrollmentsList;
using Evaluate.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentEntity = Evaluate.Domain.Entities.People.StudentEnrollment;

namespace Evaluate.Infrastructure.Repositories;

public class EnrollmentRepository(IApplicationDbContext context) : IEnrollmentRepository
{
    public Task<bool> HasActiveEnrollmentAsync(int studentId, int academicYearId, CancellationToken cancellationToken = default) =>
        context.StudentEnrollments.AnyAsync(
            e => e.StudentId == studentId && e.AcademicYearId == academicYearId && e.Status == EnrollmentStatus.Active,
            cancellationToken);

    public Task<StudentEnrollmentEntity?> GetActiveByStudentAndYearAsync(int studentId, int academicYearId, CancellationToken cancellationToken = default) =>
        context.StudentEnrollments
            .Include(e => e.Class)
            .FirstOrDefaultAsync(
                e => e.StudentId == studentId && e.AcademicYearId == academicYearId && e.Status == EnrollmentStatus.Active,
                cancellationToken);

    public void Add(StudentEnrollmentEntity enrollment) => context.StudentEnrollments.Add(enrollment);

    public async Task<List<EnrollmentDto>> GetListAsync(int? studentId, int? academicYearId, int? classId, CancellationToken cancellationToken = default)
    {
        var query = context.StudentEnrollments
            .Include(e => e.Student)
            .Include(e => e.Class)
            .AsQueryable();

        if (studentId.HasValue)
        {
            query = query.Where(e => e.StudentId == studentId);
        }

        if (academicYearId.HasValue)
        {
            query = query.Where(e => e.AcademicYearId == academicYearId);
        }

        if (classId.HasValue)
        {
            query = query.Where(e => e.ClassId == classId);
        }

        var enrollments = await query.OrderByDescending(e => e.EnrollmentDate).ToListAsync(cancellationToken);

        return enrollments
            .Select(e => new EnrollmentDto(e.Id, e.StudentId, e.Student!.FullName, e.AcademicYearId, e.ClassId, e.Class!.ClassName, e.EnrollmentDate, e.Status))
            .ToList();
    }

    public Task<List<int>> GetActiveStudentIdsAsync(int classId, int academicYearId, CancellationToken cancellationToken = default) =>
        context.StudentEnrollments
            .Where(e => e.ClassId == classId && e.AcademicYearId == academicYearId && e.Status == EnrollmentStatus.Active)
            .Select(e => e.StudentId)
            .ToListAsync(cancellationToken);

    public Task<int> CountActiveByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default) =>
        context.StudentEnrollments
            .CountAsync(e => e.AcademicYearId == academicYearId && e.Status == EnrollmentStatus.Active, cancellationToken);
}
