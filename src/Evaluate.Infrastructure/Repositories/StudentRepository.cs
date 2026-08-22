using Evaluate.Application.Students;
using Evaluate.Application.Students.Queries.GetActiveStudentsList;
using Evaluate.Application.Students.Queries.GetStudentById;
using Evaluate.Application.Students.Queries.GetStudentsList;
using Evaluate.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using IApplicationDbContext = Evaluate.Application.Common.Interfaces.IApplicationDbContext;
using StudentEntity = Evaluate.Domain.Entities.People.Student;

namespace Evaluate.Infrastructure.Repositories;

public class StudentRepository(IApplicationDbContext context) : IStudentRepository
{
    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default) =>
        context.Students.AnyAsync(s => s.Id == id, cancellationToken);

    public Task<bool> StudentNumberExistsAsync(string studentNumber, CancellationToken cancellationToken = default) =>
        context.Students.AnyAsync(s => s.StudentNumber == studentNumber, cancellationToken);

    public Task<StudentEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Students.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public void Add(StudentEntity student) => context.Students.Add(student);

    public Task<List<StudentEntity>> GetByIdsAsync(List<int> ids, CancellationToken cancellationToken = default) =>
        context.Students.Where(s => ids.Contains(s.Id)).ToListAsync(cancellationToken);

    public async Task<StudentDetailDto?> GetDetailByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await context.Students.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (student is null)
        {
            return null;
        }

        var currentYear = await context.AcademicYears.FirstOrDefaultAsync(y => y.IsCurrent, cancellationToken);

        var enrollment = currentYear is null
            ? null
            : await context.StudentEnrollments
                .Include(e => e.Class)
                .FirstOrDefaultAsync(
                    e => e.StudentId == student.Id && e.AcademicYearId == currentYear.Id && e.Status == EnrollmentStatus.Active,
                    cancellationToken);

        return new StudentDetailDto(
            student.Id,
            student.StudentNumber,
            student.FirstName,
            student.MiddleName,
            student.LastName,
            student.DateOfBirth,
            student.Gender,
            student.Email,
            student.PhoneNumber,
            student.Address,
            student.IsActive,
            enrollment?.ClassId,
            enrollment?.Class?.ClassName);
    }

    public async Task<List<StudentDto>> GetListAsync(CancellationToken cancellationToken = default)
    {
        var students = await context.Students
            .OrderBy(s => s.LastName).ThenBy(s => s.FirstName)
            .ToListAsync(cancellationToken);

        return students
            .Select(s => new StudentDto(s.Id, s.StudentNumber, s.FullName, s.DateOfBirth, s.Gender, s.Email, s.IsActive))
            .ToList();
    }

    public async Task<List<ActiveStudentDto>> GetActiveListAsync(CancellationToken cancellationToken = default)
    {
        var currentYear = await context.AcademicYears.FirstOrDefaultAsync(y => y.IsCurrent, cancellationToken);
        if (currentYear is null)
        {
            return [];
        }

        var enrollments = await context.StudentEnrollments
            .Include(e => e.Student)
            .Include(e => e.Class)
            .Where(e => e.AcademicYearId == currentYear.Id
                && e.Status == EnrollmentStatus.Active
                && e.Student!.IsActive)
            .ToListAsync(cancellationToken);

        var currentTerm = await context.Terms.FirstOrDefaultAsync(t => t.AcademicYearId == currentYear.Id && t.IsCurrent, cancellationToken);

        var classIds = enrollments.Select(e => e.ClassId).Distinct().ToList();
        var studentIds = enrollments.Select(e => e.StudentId).ToList();

        var registeredCoursesByClass = new Dictionary<int, HashSet<int>>();
        var evaluatedCoursesByStudent = new Dictionary<int, HashSet<int>>();

        if (currentTerm is not null)
        {
            var teacherCourses = await context.TeacherCourses
                .Where(tc => classIds.Contains(tc.ClassId) && tc.AcademicYearId == currentYear.Id && tc.IsActive)
                .Select(tc => new { tc.ClassId, tc.CourseId })
                .ToListAsync(cancellationToken);
            registeredCoursesByClass = teacherCourses
                .GroupBy(tc => tc.ClassId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.CourseId).ToHashSet());

            var finalizedPairs = await context.Evaluations
                .Where(e => studentIds.Contains(e.StudentId) && e.TermId == currentTerm.Id && e.Status == EvaluationStatus.Finalized)
                .Select(e => new { e.StudentId, e.CourseId })
                .ToListAsync(cancellationToken);
            evaluatedCoursesByStudent = finalizedPairs
                .GroupBy(p => p.StudentId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.CourseId).ToHashSet());
        }

        return enrollments
            .Select(e =>
            {
                var registered = registeredCoursesByClass.GetValueOrDefault(e.ClassId, []);
                var evaluated = evaluatedCoursesByStudent.GetValueOrDefault(e.StudentId, []);
                var reportCardReady = registered.Count > 0 && registered.All(evaluated.Contains);

                return new ActiveStudentDto(
                    e.Student!.Id,
                    e.Student.StudentNumber,
                    e.Student.FullName,
                    e.Student.DateOfBirth,
                    e.Student.Gender,
                    e.Student.Email,
                    e.ClassId,
                    e.Class!.ClassName,
                    e.EnrollmentDate,
                    reportCardReady);
            })
            .OrderBy(s => s.FullName)
            .ToList();
    }

    public Task<Dictionary<int, string>> GetNamesByIdsAsync(List<int> studentIds, CancellationToken cancellationToken = default) =>
        context.Students
            .Where(s => studentIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, s => s.FullName, cancellationToken);
}
