using Evaluate.Application.EvaluationResults.Queries.GetEvaluationResultsList;
using Evaluate.Application.Evaluations.Queries.GetEvaluationsList;
using EvaluationEntity = Evaluate.Domain.Entities.Evaluations.Evaluation;

namespace Evaluate.Application.Evaluations;

public interface IEvaluationRepository
{
    Task<bool> ExistsAsync(int studentId, int courseId, int termId, CancellationToken cancellationToken = default);

    void Add(EvaluationEntity evaluation);

    /// <summary>Each evaluation's <see cref="Domain.Entities.Evaluations.EvaluationResult"/> children are eagerly loaded.</summary>
    Task<List<EvaluationEntity>> GetListAsync(EvaluationsFilterSpecification specification, CancellationToken cancellationToken = default);

    /// <summary>Finalized evaluations for one student/term, with results and each result's topic eagerly loaded.</summary>
    Task<List<EvaluationEntity>> GetFinalizedForStudentAndTermAsync(int studentId, int termId, CancellationToken cancellationToken = default);

    /// <summary>Every (StudentId, CourseId) pair with a finalized evaluation, restricted to the given courses/term/students.</summary>
    Task<List<(int StudentId, int CourseId)>> GetFinalizedStudentCoursePairsAsync(
        List<int> courseIds, int termId, List<int> studentIds, CancellationToken cancellationToken = default);

    Task<List<EvaluationResultDto>> GetResultsByEvaluationIdAsync(int evaluationId, CancellationToken cancellationToken = default);
}
