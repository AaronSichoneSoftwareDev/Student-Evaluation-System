using Evaluate.Application.Common.Exceptions;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Application.Evaluations.Queries.GetStudentReportCard;

public class GetStudentReportCardQueryHandler(IApplicationDbContext context) : IRequestHandler<GetStudentReportCardQuery, ReportCardDto>
{
    public async Task<ReportCardDto> Handle(GetStudentReportCardQuery request, CancellationToken cancellationToken)
    {
        var student = await context.Students.FirstOrDefaultAsync(s => s.Id == request.StudentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.People.Student), request.StudentId);

        var term = request.TermId.HasValue
            ? await context.Terms.FirstOrDefaultAsync(t => t.Id == request.TermId, cancellationToken)
            : await ResolveCurrentTermAsync(cancellationToken);

        var academicYearName = "—";
        var termName = "No current term set";
        var className = "—";
        var courses = new List<CourseReportDto>();
        var isComplete = false;

        if (term is not null)
        {
            var academicYear = await context.AcademicYears.FirstOrDefaultAsync(y => y.Id == term.AcademicYearId, cancellationToken);
            academicYearName = academicYear?.YearName ?? "—";
            termName = term.TermName;

            var enrollment = await context.StudentEnrollments
                .Include(e => e.Class)
                .FirstOrDefaultAsync(
                    e => e.StudentId == student.Id && e.AcademicYearId == term.AcademicYearId && e.Status == EnrollmentStatus.Active,
                    cancellationToken);
            className = enrollment?.Class?.ClassName ?? "—";

            var evaluations = await context.Evaluations
                .Include(e => e.Results)
                .ThenInclude(r => r.Topic)
                .Where(e => e.StudentId == student.Id && e.TermId == term.Id && e.Status == EvaluationStatus.Finalized)
                .ToListAsync(cancellationToken);

            var courseIds = evaluations.Select(e => e.CourseId).Distinct().ToList();
            var courseNames = await context.Courses
                .Where(c => courseIds.Contains(c.Id))
                .ToDictionaryAsync(c => c.Id, c => c.CourseName, cancellationToken);

            courses = evaluations
                .Select(e => new CourseReportDto(
                    courseNames.GetValueOrDefault(e.CourseId, "Unknown"),
                    e.FinalPercentage,
                    e.FinalGrade,
                    e.Comments,
                    e.Results.Select(r => new TopicResultDto(r.Topic?.TopicName ?? "Unknown", r.Score, r.Comment)).ToList()))
                .ToList();

            if (enrollment is not null)
            {
                var registeredCourseIds = await context.TeacherCourses
                    .Where(tc => tc.ClassId == enrollment.ClassId && tc.AcademicYearId == term.AcademicYearId && tc.IsActive)
                    .Select(tc => tc.CourseId)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                isComplete = registeredCourseIds.Count > 0 && registeredCourseIds.All(courseIds.Contains);
            }
        }

        return new ReportCardDto(student.FullName, student.StudentNumber, className, academicYearName, termName, courses, isComplete);
    }

    private async Task<Domain.Entities.Academic.Term?> ResolveCurrentTermAsync(CancellationToken cancellationToken)
    {
        var currentYear = await context.AcademicYears.FirstOrDefaultAsync(y => y.IsCurrent, cancellationToken);
        if (currentYear is null)
        {
            return null;
        }

        return await context.Terms.FirstOrDefaultAsync(t => t.AcademicYearId == currentYear.Id && t.IsCurrent, cancellationToken);
    }
}
