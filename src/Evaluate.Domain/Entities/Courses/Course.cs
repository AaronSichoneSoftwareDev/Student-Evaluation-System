using Evaluate.Domain.Common;

namespace Evaluate.Domain.Entities.Courses;

public class Course : BaseAuditableEntity
{
    public string CourseCode { get; private set; } = string.Empty;
    public string CourseName { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    public ICollection<Topic> Topics { get; private set; } = new List<Topic>();

    private Course()
    {
    }

    private Course(string courseCode, string courseName, string? description)
    {
        CourseCode = courseCode;
        CourseName = courseName;
        Description = description;
    }

    public static Course Create(string courseCode, string courseName, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(courseCode))
        {
            throw new ArgumentException("Course code is required.", nameof(courseCode));
        }

        if (string.IsNullOrWhiteSpace(courseName))
        {
            throw new ArgumentException("Course name is required.", nameof(courseName));
        }

        return new Course(courseCode.Trim().ToUpperInvariant(), courseName.Trim(), description?.Trim());
    }

    public void Update(string courseName, string? description)
    {
        if (string.IsNullOrWhiteSpace(courseName))
        {
            throw new ArgumentException("Course name is required.", nameof(courseName));
        }

        CourseName = courseName.Trim();
        Description = description?.Trim();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}
