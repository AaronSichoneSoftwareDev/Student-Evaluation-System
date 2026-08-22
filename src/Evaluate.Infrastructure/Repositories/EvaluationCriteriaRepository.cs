using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.EvaluationCriteria;
using Evaluate.Application.EvaluationCriteria.Queries.GetEvaluationCriteriaList;
using Microsoft.EntityFrameworkCore;
using EvaluationCriteriaEntity = Evaluate.Domain.Entities.Evaluations.EvaluationCriteria;

namespace Evaluate.Infrastructure.Repositories;

public class EvaluationCriteriaRepository(IApplicationDbContext context) : IEvaluationCriteriaRepository
{
    public Task<decimal> GetActiveWeightSumAsync(int courseId, CancellationToken cancellationToken = default) =>
        context.EvaluationCriteria
            .Where(c => c.CourseId == courseId && c.IsActive)
            .SumAsync(c => c.Weight, cancellationToken);

    public void Add(EvaluationCriteriaEntity criteria) => context.EvaluationCriteria.Add(criteria);

    public Task<List<EvaluationCriteriaDto>> GetListAsync(int? courseId, CancellationToken cancellationToken = default)
    {
        var query = context.EvaluationCriteria.AsQueryable();
        if (courseId.HasValue)
        {
            query = query.Where(c => c.CourseId == courseId);
        }

        return query
            .OrderBy(c => c.CriteriaName)
            .Select(c => new EvaluationCriteriaDto(c.Id, c.CourseId, c.CriteriaName, c.Description, c.MaxScore, c.Weight, c.IsActive))
            .ToListAsync(cancellationToken);
    }
}
