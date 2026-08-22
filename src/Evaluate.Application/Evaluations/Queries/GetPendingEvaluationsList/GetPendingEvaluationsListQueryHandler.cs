using Evaluate.Application.AcademicYears;
using Evaluate.Application.Classes;
using Evaluate.Application.Enrollments;
using Evaluate.Application.TeacherCourses;
using Evaluate.Application.Terms;
using MediatR;
using IStudentRepository = Evaluate.Application.Students.IStudentRepository;

namespace Evaluate.Application.Evaluations.Queries.GetPendingEvaluationsList;

/// <summary>Answers "which students still need evaluating?" for one teacher's class in the
/// current term. A teacher may be assigned more than one subject for the same class, so a
/// student counts as pending if they're missing a finalized evaluation in *any* of those
/// subjects — which one gets resolved is left to the teacher when they click Evaluate.</summary>
public class GetPendingEvaluationsListQueryHandler(
    IAcademicYearRepository academicYears,
    ITermRepository terms,
    ITeacherCourseRepository teacherCourses,
    IEnrollmentRepository enrollments,
    IEvaluationRepository evaluations,
    IClassRepository classes,
    IStudentRepository students) : IRequestHandler<GetPendingEvaluationsListQuery, PendingEvaluationsResult>
{
    public async Task<PendingEvaluationsResult> Handle(GetPendingEvaluationsListQuery request, CancellationToken cancellationToken)
    {
        var currentYear = await academicYears.GetCurrentAsync(cancellationToken);
        if (currentYear is null)
        {
            return new PendingEvaluationsResult(false, null, null, false, [], []);
        }

        var currentTerm = await terms.GetCurrentAsync(currentYear.Id, cancellationToken);
        if (currentTerm is null)
        {
            return new PendingEvaluationsResult(true, currentYear.YearName, null, false, [], []);
        }

        var assignments = await teacherCourses.GetActiveForTeacherAndClassAsync(request.TeacherUserId, request.ClassId, currentYear.Id, cancellationToken);
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

        var enrolledStudentIds = await enrollments.GetActiveStudentIdsAsync(request.ClassId, currentYear.Id, cancellationToken);

        var evaluatedPairs = await evaluations.GetFinalizedStudentCoursePairsAsync(courseIds, currentTerm.Id, enrolledStudentIds, cancellationToken);
        var evaluatedSet = evaluatedPairs.ToHashSet();

        var pendingStudentIds = enrolledStudentIds
            .Where(studentId => courseIds.Any(courseId => !evaluatedSet.Contains((studentId, courseId))))
            .ToList();

        var schoolClass = (await classes.GetByIdAsync(request.ClassId, cancellationToken))!;

        var pendingStudents = await students.GetByIdsAsync(pendingStudentIds, cancellationToken);

        var studentDtos = pendingStudents
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

        return new PendingEvaluationsResult(true, currentYear.YearName, currentTerm.TermName, true, availableCourses, studentDtos);
    }
}
