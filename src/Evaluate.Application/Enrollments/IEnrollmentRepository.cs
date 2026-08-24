using Evaluate.Application.Enrollments.Queries.GetEnrollmentsList;
using StudentEnrollmentEntity = Evaluate.Domain.Entities.People.StudentEnrollment;

namespace Evaluate.Application.Enrollments;

public interface IEnrollmentRepository
{
    Task<bool> HasActiveEnrollmentAsync(int studentId, int academicYearId, CancellationToken cancellationToken = default);

    /// <summary>The active enrollment's <see cref="Domain.Entities.Academic.SchoolClass"/> navigation is eagerly loaded.</summary>
    Task<StudentEnrollmentEntity?> GetActiveByStudentAndYearAsync(int studentId, int academicYearId, CancellationToken cancellationToken = default);

    void Add(StudentEnrollmentEntity enrollment);

    Task<List<EnrollmentDto>> GetListAsync(int? studentId, int? academicYearId, int? classId, CancellationToken cancellationToken = default);

    Task<List<int>> GetActiveStudentIdsAsync(int classId, int academicYearId, CancellationToken cancellationToken = default);

    /// <summary>Total pupils actively enrolled (and therefore eligible for evaluation) across
    /// every class for the given academic year — a single indexed COUNT, not a full row load.</summary>
    Task<int> CountActiveByAcademicYearAsync(int academicYearId, CancellationToken cancellationToken = default);
}
