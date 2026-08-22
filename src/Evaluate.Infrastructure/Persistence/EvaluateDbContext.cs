using Evaluate.Application.Common.Interfaces;
using Evaluate.Domain.Entities.Academic;
using Evaluate.Domain.Entities.Courses;
using Evaluate.Domain.Entities.People;
using Evaluate.Domain.Entities.System;
using Evaluate.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EvaluationCriteriaEntity = Evaluate.Domain.Entities.Evaluations.EvaluationCriteria;
using EvaluationEntity = Evaluate.Domain.Entities.Evaluations.Evaluation;
using EvaluationResultEntity = Evaluate.Domain.Entities.Evaluations.EvaluationResult;

// Dashboard demo entities kept separate from the real school-management model below.
using DashboardStudent = Evaluate.Domain.Entities.Dashboard.Student;
using DashboardInstructor = Evaluate.Domain.Entities.Dashboard.Instructor;
using DashboardEvaluation = Evaluate.Domain.Entities.Dashboard.Evaluation;
using DashboardSubmission = Evaluate.Domain.Entities.Dashboard.Submission;
using DashboardActivityFeedItem = Evaluate.Domain.Entities.Dashboard.ActivityFeedItem;
using DashboardInboxMessage = Evaluate.Domain.Entities.Dashboard.InboxMessage;

namespace Evaluate.Infrastructure.Persistence;

public class EvaluateDbContext(DbContextOptions<EvaluateDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options), IApplicationDbContext
{
    // Dashboard demo data (untouched — feeds the dashboard's seeded visuals only).
    public DbSet<DashboardStudent> Students => Set<DashboardStudent>();
    public DbSet<DashboardInstructor> Instructors => Set<DashboardInstructor>();
    public DbSet<DashboardEvaluation> Evaluations => Set<DashboardEvaluation>();
    public DbSet<DashboardSubmission> Submissions => Set<DashboardSubmission>();
    public DbSet<DashboardActivityFeedItem> ActivityFeedItems => Set<DashboardActivityFeedItem>();
    public DbSet<DashboardInboxMessage> InboxMessages => Set<DashboardInboxMessage>();

    // Real school-management model (IApplicationDbContext).
    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();
    public DbSet<Term> Terms => Set<Term>();
    public DbSet<SchoolClass> Classes => Set<SchoolClass>();
    DbSet<Domain.Entities.People.Student> IApplicationDbContext.Students => Set<Domain.Entities.People.Student>();
    public DbSet<StudentEnrollment> StudentEnrollments => Set<StudentEnrollment>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<TeacherCourse> TeacherCourses => Set<TeacherCourse>();
    public DbSet<EvaluationCriteriaEntity> EvaluationCriteria => Set<EvaluationCriteriaEntity>();
    DbSet<EvaluationEntity> IApplicationDbContext.Evaluations => Set<EvaluationEntity>();
    public DbSet<EvaluationResultEntity> EvaluationResults => Set<EvaluationResultEntity>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Ignore<Evaluate.Domain.Common.BaseEvent>();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EvaluateDbContext).Assembly);
    }
}
