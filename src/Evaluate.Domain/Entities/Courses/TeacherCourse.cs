using Evaluate.Domain.Common;

namespace Evaluate.Domain.Entities.Courses;

/// <summary>
/// Assigns a teacher (an ApplicationUser id from Identity, kept as a plain string here so
/// Domain has no dependency on the Infrastructure-owned Identity types) to a course/class
/// for a given academic year.
/// </summary>
public class TeacherCourse : BaseAuditableEntity
{
    public string TeacherUserId { get; private set; } = string.Empty;
    public int CourseId { get; private set; }
    public Course? Course { get; private set; }
    public int AcademicYearId { get; private set; }
    public int ClassId { get; private set; }
    public bool IsActive { get; private set; } = true;

    private TeacherCourse()
    {
    }

    private TeacherCourse(string teacherUserId, int courseId, int academicYearId, int classId)
    {
        TeacherUserId = teacherUserId;
        CourseId = courseId;
        AcademicYearId = academicYearId;
        ClassId = classId;
    }

    public static TeacherCourse Assign(string teacherUserId, int courseId, int academicYearId, int classId)
    {
        if (string.IsNullOrWhiteSpace(teacherUserId))
        {
            throw new ArgumentException("Teacher user id is required.", nameof(teacherUserId));
        }

        return new TeacherCourse(teacherUserId, courseId, academicYearId, classId);
    }

    public void Deactivate() => IsActive = false;
}
