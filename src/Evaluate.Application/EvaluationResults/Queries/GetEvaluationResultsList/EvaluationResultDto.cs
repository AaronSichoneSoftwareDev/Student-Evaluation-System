namespace Evaluate.Application.EvaluationResults.Queries.GetEvaluationResultsList;

public record EvaluationResultDto(int Id, int EvaluationId, int TopicId, string TopicName, decimal Score, string? Comment);
