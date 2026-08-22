using Evaluate.Application.Common.Specifications;
using EvaluationEntity = Evaluate.Domain.Entities.Evaluations.Evaluation;

namespace Evaluate.Application.Evaluations.Queries.GetEvaluationsList;

/// <summary>Composes every optional filter for the evaluations list into a single
/// reusable specification instead of chaining conditional `.Where` calls in the handler.</summary>
public class EvaluationsFilterSpecification(int? studentId, int? courseId, int? termId, int? academicYearId)
    : BaseSpecification<EvaluationEntity>(e =>
        (!studentId.HasValue || e.StudentId == studentId) &&
        (!courseId.HasValue || e.CourseId == courseId) &&
        (!termId.HasValue || e.TermId == termId) &&
        (!academicYearId.HasValue || e.AcademicYearId == academicYearId));
