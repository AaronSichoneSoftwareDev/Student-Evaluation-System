using Evaluate.Application.AcademicYears;
using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Courses;
using Evaluate.Application.Enrollments;
using Evaluate.Application.TeacherCourses;
using Evaluate.Application.Terms;
using MediatR;
using IStudentRepository = Evaluate.Application.Students.IStudentRepository;
using TermEntity = Evaluate.Domain.Entities.Academic.Term;

namespace Evaluate.Application.Evaluations.Queries.GetStudentReportCard;

public class GetStudentReportCardQueryHandler(
    IStudentRepository students,
    ITermRepository terms,
    IAcademicYearRepository academicYears,
    IEnrollmentRepository enrollments,
    IEvaluationRepository evaluations,
    ICourseRepository courses,
    ITeacherCourseRepository teacherCourses) : IRequestHandler<GetStudentReportCardQuery, ReportCardDto>
{
    public async Task<ReportCardDto> Handle(GetStudentReportCardQuery request, CancellationToken cancellationToken)
    {
        var student = await students.GetByIdAsync(request.StudentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.People.Student), request.StudentId);

        var term = request.TermId.HasValue
            ? await terms.GetByIdAsync(request.TermId.Value, cancellationToken)
            : await ResolveCurrentTermAsync(cancellationToken);

        var academicYearName = "—";
        var termName = "No current term set";
        var className = "—";
        var reportCourses = new List<CourseReportDto>();
        var isComplete = false;

        if (term is not null)
        {
            var academicYear = await academicYears.GetByIdAsync(term.AcademicYearId, cancellationToken);
            academicYearName = academicYear?.YearName ?? "—";
            termName = term.TermName;

            var enrollment = await enrollments.GetActiveByStudentAndYearAsync(student.Id, term.AcademicYearId, cancellationToken);
            className = enrollment?.Class?.ClassName ?? "—";

            var finalizedEvaluations = await evaluations.GetFinalizedForStudentAndTermAsync(student.Id, term.Id, cancellationToken);

            var courseIds = finalizedEvaluations.Select(e => e.CourseId).Distinct().ToList();
            var courseNames = await courses.GetNamesByIdsAsync(courseIds, cancellationToken);

            reportCourses = finalizedEvaluations
                .Select(e => new CourseReportDto(
                    courseNames.GetValueOrDefault(e.CourseId, "Unknown"),
                    e.FinalPercentage,
                    e.FinalGrade,
                    e.Comments,
                    e.Results.Select(r => new TopicResultDto(r.Topic?.TopicName ?? "Unknown", r.Score, r.Comment)).ToList()))
                .ToList();

            if (enrollment is not null)
            {
                var registeredCourses = await teacherCourses.GetActiveForClassAsync(enrollment.ClassId, term.AcademicYearId, cancellationToken);
                var registeredCourseIds = registeredCourses.Select(tc => tc.CourseId).Distinct().ToList();
                isComplete = registeredCourseIds.Count > 0 && registeredCourseIds.All(courseIds.Contains);
            }
        }

        return new ReportCardDto(student.FullName, student.StudentNumber, className, academicYearName, termName, reportCourses, isComplete);
    }

    private async Task<TermEntity?> ResolveCurrentTermAsync(CancellationToken cancellationToken)
    {
        var currentYear = await academicYears.GetCurrentAsync(cancellationToken);
        if (currentYear is null)
        {
            return null;
        }

        return await terms.GetCurrentAsync(currentYear.Id, cancellationToken);
    }
}
