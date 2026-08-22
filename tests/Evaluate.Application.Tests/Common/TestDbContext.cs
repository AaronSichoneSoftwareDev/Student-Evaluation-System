using Evaluate.Application.Common.Interfaces;
using Evaluate.Domain.Entities.Academic;
using Evaluate.Domain.Entities.Courses;
using Evaluate.Domain.Entities.People;
using Evaluate.Domain.Entities.System;
using Microsoft.EntityFrameworkCore;
using EvaluationCriteriaEntity = Evaluate.Domain.Entities.Evaluations.EvaluationCriteria;
using EvaluationEntity = Evaluate.Domain.Entities.Evaluations.Evaluation;
using EvaluationResultEntity = Evaluate.Domain.Entities.Evaluations.EvaluationResult;

namespace Evaluate.Application.Tests.Common;

/// <summary>Minimal EF Core InMemory-backed <see cref="IApplicationDbContext"/> for
/// exercising command/query handlers without spinning up Infrastructure/SQLite.</summary>
public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();
    public DbSet<Term> Terms => Set<Term>();
    public DbSet<SchoolClass> Classes => Set<SchoolClass>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<StudentEnrollment> StudentEnrollments => Set<StudentEnrollment>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<TeacherCourse> TeacherCourses => Set<TeacherCourse>();
    public DbSet<EvaluationCriteriaEntity> EvaluationCriteria => Set<EvaluationCriteriaEntity>();
    public DbSet<EvaluationEntity> Evaluations => Set<EvaluationEntity>();
    public DbSet<EvaluationResultEntity> EvaluationResults => Set<EvaluationResultEntity>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Evaluate.Domain.Common.BaseEvent>();
        base.OnModelCreating(modelBuilder);
    }

    public static TestDbContext Create()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new TestDbContext(options);
    }
}
