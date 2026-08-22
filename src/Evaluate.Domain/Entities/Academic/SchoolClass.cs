using Evaluate.Domain.Common;

namespace Evaluate.Domain.Entities.Academic;

public class SchoolClass : BaseAuditableEntity
{
    public string ClassName { get; private set; } = string.Empty;
    public string GradeLevel { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    private SchoolClass()
    {
    }

    private SchoolClass(string className, string gradeLevel, string? description)
    {
        ClassName = className;
        GradeLevel = gradeLevel;
        Description = description;
    }

    public static SchoolClass Create(string className, string gradeLevel, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(className))
        {
            throw new ArgumentException("Class name is required.", nameof(className));
        }

        if (string.IsNullOrWhiteSpace(gradeLevel))
        {
            throw new ArgumentException("Grade level is required.", nameof(gradeLevel));
        }

        return new SchoolClass(className.Trim(), gradeLevel.Trim(), description?.Trim());
    }

    public void Deactivate() => IsActive = false;
}
