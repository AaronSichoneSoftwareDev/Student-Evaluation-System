using Evaluate.Application.Common.Interfaces;
using Evaluate.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Application.Students.Queries.GetActiveStudentsList;

public class GetActiveStudentsListQueryHandler(IApplicationDbContext context) : IRequestHandler<GetActiveStudentsListQuery, List<ActiveStudentDto>>
{
    public async Task<List<ActiveStudentDto>> Handle(GetActiveStudentsListQuery request, CancellationToken cancellationToken)
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
}
