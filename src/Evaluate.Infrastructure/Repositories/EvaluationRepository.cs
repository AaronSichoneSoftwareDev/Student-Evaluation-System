using Evaluate.Application.Common.Specifications;
using Evaluate.Application.EvaluationResults.Queries.GetEvaluationResultsList;
using Evaluate.Application.Evaluations;
using Evaluate.Application.Evaluations.Queries.GetEvaluationsList;
using Evaluate.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using EvaluationEntity = Evaluate.Domain.Entities.Evaluations.Evaluation;
using IApplicationDbContext = Evaluate.Application.Common.Interfaces.IApplicationDbContext;

namespace Evaluate.Infrastructure.Repositories;

public class EvaluationRepository(IApplicationDbContext context) : IEvaluationRepository
{
    public Task<bool> ExistsAsync(int studentId, int courseId, int termId, CancellationToken cancellationToken = default) =>
        context.Evaluations.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId && e.TermId == termId, cancellationToken);

    public void Add(EvaluationEntity evaluation) => context.Evaluations.Add(evaluation);

    public Task<List<EvaluationEntity>> GetListAsync(EvaluationsFilterSpecification specification, CancellationToken cancellationToken = default) =>
        context.Evaluations
            .Include(e => e.Results)
            .Apply(specification)
            .OrderByDescending(e => e.EvaluationDate)
            .ToListAsync(cancellationToken);

    public Task<List<EvaluationEntity>> GetFinalizedForStudentAndTermAsync(int studentId, int termId, CancellationToken cancellationToken = default) =>
        context.Evaluations
            .Include(e => e.Results)
            .ThenInclude(r => r.Topic)
            .Where(e => e.StudentId == studentId && e.TermId == termId && e.Status == EvaluationStatus.Finalized)
            .ToListAsync(cancellationToken);

    public async Task<List<(int StudentId, int CourseId)>> GetFinalizedStudentCoursePairsAsync(
        List<int> courseIds, int termId, List<int> studentIds, CancellationToken cancellationToken = default)
    {
        var pairs = await context.Evaluations
            .Where(e => courseIds.Contains(e.CourseId) && e.TermId == termId && e.Status == EvaluationStatus.Finalized && studentIds.Contains(e.StudentId))
            .Select(e => new { e.StudentId, e.CourseId })
            .ToListAsync(cancellationToken);

        return pairs.Select(p => (p.StudentId, p.CourseId)).ToList();
    }

    public async Task<List<EvaluationResultDto>> GetResultsByEvaluationIdAsync(int evaluationId, CancellationToken cancellationToken = default)
    {
        var results = await context.EvaluationResults
            .Include(r => r.Topic)
            .Where(r => r.EvaluationId == evaluationId)
            .ToListAsync(cancellationToken);

        return results
            .Select(r => new EvaluationResultDto(r.Id, r.EvaluationId, r.TopicId, r.Topic!.TopicName, r.Score, r.Comment))
            .ToList();
    }
}
