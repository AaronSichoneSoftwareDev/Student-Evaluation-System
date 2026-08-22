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
/// Application's own view of the persistence layer (Dependency Inversion): it depends on this
/// interface, not on EF Core or <c>EvaluateDbContext</c> directly. Doubles as the Unit of Work
/// via <see cref="SaveChangesAsync"/>, so a separate generic repository layer on top of EF Core's
/// own repository/UoW implementation would just be redundant ceremony.
/// </summary>
public interface IApplicationDbContext
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

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
