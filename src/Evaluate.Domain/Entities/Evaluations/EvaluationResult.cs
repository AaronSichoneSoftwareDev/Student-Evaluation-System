using Evaluate.Domain.Common;
using Evaluate.Domain.Entities.Courses;

namespace Evaluate.Domain.Entities.Evaluations;

public class EvaluationResult : BaseAuditableEntity
{
    public int EvaluationId { get; private set; }
    public Evaluation? Evaluation { get; private set; }
    public int TopicId { get; private set; }
    public Topic? Topic { get; private set; }
    public decimal Score { get; private set; }
    public string? Comment { get; private set; }

    private EvaluationResult()
    {
    }

    private EvaluationResult(int evaluationId, int topicId, decimal score, string? comment)
    {
        EvaluationId = evaluationId;
        TopicId = topicId;
        Score = score;
        Comment = comment;
    }

    public static EvaluationResult Create(int evaluationId, int topicId, decimal score, string? comment)
    {
        if (score is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(score), "Score must be between 0 and 100.");
        }

        return new EvaluationResult(evaluationId, topicId, score, string.IsNullOrWhiteSpace(comment) ? null : comment.Trim());
    }
}
