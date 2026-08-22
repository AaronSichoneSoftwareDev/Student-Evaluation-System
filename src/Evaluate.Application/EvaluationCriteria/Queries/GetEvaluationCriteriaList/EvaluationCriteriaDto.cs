namespace Evaluate.Application.EvaluationCriteria.Queries.GetEvaluationCriteriaList;

public record EvaluationCriteriaDto(int Id, int CourseId, string CriteriaName, string? Description, decimal MaxScore, decimal Weight, bool IsActive);
