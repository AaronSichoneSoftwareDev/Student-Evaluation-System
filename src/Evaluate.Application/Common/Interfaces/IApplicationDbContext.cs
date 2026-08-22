using Evaluate.Domain.Entities.Academic;
using Evaluate.Domain.Entities.Courses;
using Evaluate.Domain.Entities.People;
using Evaluate.Domain.Entities.System;
using Microsoft.EntityFrameworkCore;
using EvaluationCriteriaEntity = Evaluate.Domain.Entities.Evaluations.EvaluationCriteria;
using EvaluationEntity = Evaluate.Domain.Entities.Evaluations.Evaluation;
using EvaluationResultEntity = Evaluate.Domain.Entities.Evaluations.EvaluationResult;

namespace Evaluate.Application.Common.Interfaces;

/// <summary>
/// EF Core's own view of the persistence layer. Application code no longer depends on this
/// directly — each aggregate has its own repository interface (<c>IStudentRepository</c>,
/// <c>IEvaluationRepository</c>, etc.) that command/query handlers depend on instead. This
/// interface is now consumed only by those repository implementations (in Infrastructure) and
/// by <see cref="IUnitOfWork"/>, which every repository shares to commit changes together.
/// </summary>
public interface IApplicationDbContext : IUnitOfWork
{
    DbSet<AcademicYear> AcademicYears { get; }
    DbSet<Term> Terms { get; }
    DbSet<SchoolClass> Classes { get; }
    DbSet<Student> Students { get; }
    DbSet<StudentEnrollment> StudentEnrollments { get; }
    DbSet<Course> Courses { get; }
    DbSet<Topic> Topics { get; }
    DbSet<TeacherCourse> TeacherCourses { get; }
    DbSet<EvaluationCriteriaEntity> EvaluationCriteria { get; }
    DbSet<EvaluationEntity> Evaluations { get; }
    DbSet<EvaluationResultEntity> EvaluationResults { get; }
    DbSet<AuditLog> AuditLogs { get; }
}
