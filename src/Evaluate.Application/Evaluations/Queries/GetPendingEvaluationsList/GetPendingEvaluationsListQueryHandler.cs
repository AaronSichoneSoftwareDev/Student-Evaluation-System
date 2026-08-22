using Evaluate.Application.Common.Interfaces;
using Evaluate.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Application.Evaluations.Queries.GetPendingEvaluationsList;

/// <summary>Answers "which students still need evaluating?" for one teacher's class in the
/// current term. A teacher may be assigned more than one subject for the same class, so a
/// student counts as pending if they're missing a finalized evaluation in *any* of those
/// subjects — which one gets resolved is left to the teacher when they click Evaluate.</summary>
public class GetPendingEvaluationsListQueryHandler(IApplicationDbContext context) : IRequestHandler<GetPendingEvaluationsListQuery, PendingEvaluationsResult>
{
    public async Task<PendingEvaluationsResult> Handle(GetPendingEvaluationsListQuery request, CancellationToken cancellationToken)
    {
        var currentYear = await context.AcademicYears.FirstOrDefaultAsync(y => y.IsCurrent, cancellationToken);
        if (currentYear is null)
        {
            return new PendingEvaluationsResult(false, null, null, false, [], []);
        }

        var currentTerm = await context.Terms.FirstOrDefaultAsync(t => t.AcademicYearId == currentYear.Id && t.IsCurrent, cancellationToken);
        if (currentTerm is null)
        {
            return new PendingEvaluationsResult(true, currentYear.YearName, null, false, [], []);
        }

        var assignments = await context.TeacherCourses
            .Include(tc => tc.Course)
            .Where(
                tc => tc.TeacherUserId == request.TeacherUserId && tc.ClassId == request.ClassId
                    && tc.AcademicYearId == currentYear.Id && tc.IsActive)
            .ToListAsync(cancellationToken);
        if (assignments.Count == 0)
        {
            return new PendingEvaluationsResult(true, currentYear.YearName, currentTerm.TermName, false, [], []);
        }

        var availableCourses = assignments
            .Select(a => new CourseOptionDto(a.CourseId, a.Course!.CourseName))
            .DistinctBy(c => c.CourseId)
            .OrderBy(c => c.CourseName)
            .ToList();
        var courseIds = availableCourses.Select(c => c.CourseId).ToList();

        var enrolledStudentIds = await context.StudentEnrollments
            .Where(e => e.ClassId == request.ClassId && e.AcademicYearId == currentYear.Id && e.Status == EnrollmentStatus.Active)
            .Select(e => e.StudentId)
            .ToListAsync(cancellationToken);

        var evaluatedPairs = await context.Evaluations
            .Where(e => courseIds.Contains(e.CourseId) && e.TermId == currentTerm.Id && e.Status == EvaluationStatus.Finalized && enrolledStudentIds.Contains(e.StudentId))
            .Select(e => new { e.StudentId, e.CourseId })
            .ToListAsync(cancellationToken);
        var evaluatedSet = evaluatedPairs.Select(p => (p.StudentId, p.CourseId)).ToHashSet();

        var pendingStudentIds = enrolledStudentIds
            .Where(studentId => courseIds.Any(courseId => !evaluatedSet.Contains((studentId, courseId))))
            .ToList();

        var schoolClass = await context.Classes.FirstAsync(c => c.Id == request.ClassId, cancellationToken);

        var pendingStudents = await context.Students
            .Where(s => pendingStudentIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        var students = pendingStudents
            .Select(s => new PendingEvaluationDto(
                s.Id,
                s.StudentNumber,
                s.FullName,
                request.ClassId,
                schoolClass.ClassName,
                currentYear.Id,
                currentTerm.Id,
                currentTerm.TermName))
            .OrderBy(s => s.StudentName)
            .ToList();

        return new PendingEvaluationsResult(true, currentYear.YearName, currentTerm.TermName, true, availableCourses, students);
    }
}
