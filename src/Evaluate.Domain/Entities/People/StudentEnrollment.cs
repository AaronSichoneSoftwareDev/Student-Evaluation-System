using Evaluate.Domain.Common;
using Evaluate.Domain.Entities.Academic;
using Evaluate.Domain.Enums;
using Evaluate.Domain.Events;

namespace Evaluate.Domain.Entities.People;

public class StudentEnrollment : BaseAuditableEntity
{
    public int StudentId { get; private set; }
    public Student? Student { get; private set; }
    public int AcademicYearId { get; private set; }
    public AcademicYear? AcademicYear { get; private set; }
    public int ClassId { get; private set; }
    public SchoolClass? Class { get; private set; }
    public DateOnly EnrollmentDate { get; private set; }
    public EnrollmentStatus Status { get; private set; }

    private StudentEnrollment()
    {
    }

    private StudentEnrollment(int studentId, int academicYearId, int classId, DateOnly enrollmentDate)
    {
        StudentId = studentId;
        AcademicYearId = academicYearId;
        ClassId = classId;
        EnrollmentDate = enrollmentDate;
        Status = EnrollmentStatus.Active;
    }

    /// <summary>Factory that also raises <see cref="StudentEnrolledEvent"/> so downstream
    /// concerns (e.g. welcome notifications, audit trails) can react without this entity
    /// knowing about them.</summary>
    public static StudentEnrollment Enroll(int studentId, int academicYearId, int classId, DateOnly enrollmentDate)
    {
        var enrollment = new StudentEnrollment(studentId, academicYearId, classId, enrollmentDate);
        enrollment.AddDomainEvent(new StudentEnrolledEvent(enrollment));
        return enrollment;
    }

    /// <summary>Moves this enrollment to a different class within the same academic
    /// year — distinct from <see cref="Transfer"/>, which ends the enrollment entirely
    /// (e.g. a transfer to another school).</summary>
    public void ReassignClass(int classId) => ClassId = classId;

    public void Withdraw() => Status = EnrollmentStatus.Withdrawn;

    public void Complete() => Status = EnrollmentStatus.Completed;

    public void Transfer() => Status = EnrollmentStatus.Transferred;
}
