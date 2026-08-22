using Evaluate.Application.Courses;
using MediatR;
using IStudentRepository = Evaluate.Application.Students.IStudentRepository;

namespace Evaluate.Application.Evaluations.Queries.GetEvaluationsList;

public class GetEvaluationsListQueryHandler(IEvaluationRepository evaluations, IStudentRepository students, ICourseRepository courses) : IRequestHandler<GetEvaluationsListQuery, List<EvaluationDto>>
{
    public async Task<List<EvaluationDto>> Handle(GetEvaluationsListQuery request, CancellationToken cancellationToken)
    {
        var specification = new EvaluationsFilterSpecification(request.StudentId, request.CourseId, request.TermId, request.AcademicYearId);

        var results = await evaluations.GetListAsync(specification, cancellationToken);

        if (results.Count == 0)
        {
            return [];
        }

        var studentIds = results.Select(e => e.StudentId).Distinct().ToList();
        var courseIds = results.Select(e => e.CourseId).Distinct().ToList();

        var studentNames = await students.GetNamesByIdsAsync(studentIds, cancellationToken);
        var courseNames = await courses.GetNamesByIdsAsync(courseIds, cancellationToken);

        return results
            .Select(e => new EvaluationDto(
                e.Id,
                e.StudentId,
                studentNames.GetValueOrDefault(e.StudentId, "Unknown"),
                e.CourseId,
                courseNames.GetValueOrDefault(e.CourseId, "Unknown"),
                e.TermId,
                e.AcademicYearId,
                e.EvaluationDate,
                e.Status,
                e.FinalPercentage,
                e.FinalGrade))
            .ToList();
    }
}
