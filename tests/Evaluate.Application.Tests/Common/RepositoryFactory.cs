using Evaluate.Application.AcademicYears;
using Evaluate.Application.AuditLogs;
using Evaluate.Application.Classes;
using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Courses;
using Evaluate.Application.Enrollments;
using Evaluate.Application.EvaluationCriteria;
using Evaluate.Application.TeacherCourses;
using Evaluate.Application.Terms;
using Evaluate.Application.Topics;
using Infra = Evaluate.Infrastructure.Repositories;
using IEvaluationRepository = Evaluate.Application.Evaluations.IEvaluationRepository;
using IStudentRepository = Evaluate.Application.Students.IStudentRepository;

namespace Evaluate.Application.Tests.Common;

/// <summary>Builds the real EF Core repository implementations against a <see cref="TestDbContext"/>,
/// so handler tests exercise the actual query logic (not a mock) while still running in-memory.</summary>
public static class RepositoryFactory
{
    public static IAcademicYearRepository AcademicYears(IApplicationDbContext context) => new Infra.AcademicYearRepository(context);
    public static ITermRepository Terms(IApplicationDbContext context) => new Infra.TermRepository(context);
    public static IClassRepository Classes(IApplicationDbContext context) => new Infra.ClassRepository(context);
    public static ICourseRepository Courses(IApplicationDbContext context) => new Infra.CourseRepository(context);
    public static ITopicRepository Topics(IApplicationDbContext context) => new Infra.TopicRepository(context);
    public static IStudentRepository Students(IApplicationDbContext context) => new Infra.StudentRepository(context);
    public static IEnrollmentRepository Enrollments(IApplicationDbContext context) => new Infra.EnrollmentRepository(context);
    public static ITeacherCourseRepository TeacherCourses(IApplicationDbContext context) => new Infra.TeacherCourseRepository(context);
    public static IEvaluationRepository Evaluations(IApplicationDbContext context) => new Infra.EvaluationRepository(context);
    public static IEvaluationCriteriaRepository EvaluationCriteria(IApplicationDbContext context) => new Infra.EvaluationCriteriaRepository(context);
    public static IAuditLogRepository AuditLogs(IApplicationDbContext context) => new Infra.AuditLogRepository(context);
}
