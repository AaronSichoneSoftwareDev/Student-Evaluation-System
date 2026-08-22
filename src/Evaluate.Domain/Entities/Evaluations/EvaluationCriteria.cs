using Evaluate.Domain.Common;
using Evaluate.Domain.Entities.Courses;

namespace Evaluate.Domain.Entities.Evaluations;

public class EvaluationCriteria : BaseAuditableEntity
{
    public int CourseId { get; private set; }
    public Course? Course { get; private set; }
    public string CriteriaName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal MaxScore { get; private set; }
    public decimal Weight { get; private set; }
    public bool IsActive { get; private set; } = true;

    private EvaluationCriteria()
    {
    }

    private EvaluationCriteria(int courseId, string criteriaName, string? description, decimal maxScore, decimal weight)
    {
        CourseId = courseId;
        CriteriaName = criteriaName;
        Description = description;
        MaxScore = maxScore;
        Weight = weight;
    }

    public static EvaluationCriteria Create(int courseId, string criteriaName, decimal maxScore, decimal weight, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(criteriaName))
        {
            throw new ArgumentException("Criteria name is required.", nameof(criteriaName));
        }

        if (maxScore <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxScore), "Max score must be positive.");
        }

        if (weight is <= 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(weight), "Weight must be between 0 and 100.");
        }

        return new EvaluationCriteria(courseId, criteriaName.Trim(), description?.Trim(), maxScore, weight);
    }

    public void Deactivate() => IsActive = false;
}
