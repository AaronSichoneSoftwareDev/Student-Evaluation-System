using Evaluate.Domain.Common;
using Evaluate.Domain.Common.ValueObjects;
using Evaluate.Domain.Enums;
using Evaluate.Domain.Events;

namespace Evaluate.Domain.Entities.Evaluations;

public class Evaluation : BaseAuditableEntity
{
    public int StudentId { get; private set; }
    public string TeacherUserId { get; private set; } = string.Empty;
    public int CourseId { get; private set; }
    public int AcademicYearId { get; private set; }
    public int TermId { get; private set; }
    public DateOnly EvaluationDate { get; private set; }
    public string? Comments { get; private set; }
    public EvaluationStatus Status { get; private set; }
    public decimal? FinalPercentage { get; private set; }
    public string? FinalGrade { get; private set; }

    public ICollection<EvaluationResult> Results { get; private set; } = new List<EvaluationResult>();

    private Evaluation()
    {
    }

    private Evaluation(int studentId, string teacherUserId, int courseId, int academicYearId, int termId, DateOnly evaluationDate, string? comments)
    {
        StudentId = studentId;
        TeacherUserId = teacherUserId;
        CourseId = courseId;
        AcademicYearId = academicYearId;
        TermId = termId;
        EvaluationDate = evaluationDate;
        Comments = comments;
        Status = EvaluationStatus.Draft;
    }

    public static Evaluation Create(int studentId, string teacherUserId, int courseId, int academicYearId, int termId, DateOnly evaluationDate, string? comments = null)
    {
        if (string.IsNullOrWhiteSpace(teacherUserId))
        {
            throw new ArgumentException("Teacher user id is required.", nameof(teacherUserId));
        }

        return new Evaluation(studentId, teacherUserId, courseId, academicYearId, termId, evaluationDate, comments?.Trim());
    }

    /// <summary>Records a score + comment for one topic within this evaluation's course. An
    /// evaluation covers every topic of a course, not just one.</summary>
    public EvaluationResult RecordTopicResult(int topicId, decimal score, string? comment)
    {
        if (Status == EvaluationStatus.Finalized)
        {
            throw new InvalidOperationException("Cannot record results on a finalized evaluation.");
        }

        var result = EvaluationResult.Create(Id, topicId, score, comment);
        Results.Add(result);
        return result;
    }

    public void Submit()
    {
        if (Results.Count == 0)
        {
            throw new InvalidOperationException("An evaluation needs at least one recorded result before it can be submitted.");
        }

        Status = EvaluationStatus.Submitted;
    }

    /// <summary>
    /// Uses the supplied grading strategy (Strategy pattern) to turn the recorded per-topic
    /// scores into a final percentage + letter grade, then raises <see cref="EvaluationFinalizedEvent"/>.
    /// </summary>
    public void Finalize(Percentage finalPercentage, string finalGrade)
    {
        if (Status != EvaluationStatus.Submitted)
        {
            throw new InvalidOperationException("Only a submitted evaluation can be finalized.");
        }

        FinalPercentage = finalPercentage;
        FinalGrade = finalGrade;
        Status = EvaluationStatus.Finalized;
        AddDomainEvent(new EvaluationFinalizedEvent(this));
    }
}
