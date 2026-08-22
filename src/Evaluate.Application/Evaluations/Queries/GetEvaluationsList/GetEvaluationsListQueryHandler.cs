using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Specifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Application.Evaluations.Queries.GetEvaluationsList;

public class GetEvaluationsListQueryHandler(IApplicationDbContext context) : IRequestHandler<GetEvaluationsListQuery, List<EvaluationDto>>
{
    public async Task<List<EvaluationDto>> Handle(GetEvaluationsListQuery request, CancellationToken cancellationToken)
    {
        var specification = new EvaluationsFilterSpecification(request.StudentId, request.CourseId, request.TermId, request.AcademicYearId);

        var evaluations = await context.Evaluations
            .Include(e => e.Results)
            .Apply(specification)
            .OrderByDescending(e => e.EvaluationDate)
            .ToListAsync(cancellationToken);

        if (evaluations.Count == 0)
        {
            return [];
        }

        var studentIds = evaluations.Select(e => e.StudentId).Distinct().ToList();
        var courseIds = evaluations.Select(e => e.CourseId).Distinct().ToList();

        var studentNames = await context.Students
            .Where(s => studentIds.Contains(s.Id))
            .ToDictionaryAsync(s => s.Id, s => s.FullName, cancellationToken);

        var courseNames = await context.Courses
            .Where(c => courseIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.CourseName, cancellationToken);

        return evaluations
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
