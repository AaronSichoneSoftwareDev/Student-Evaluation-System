using Evaluate.Application.Common.Interfaces;
using Evaluate.Infrastructure.Identity;
using Evaluate.Infrastructure.Persistence;
using Evaluate.Infrastructure.Persistence.Interceptors;
using Evaluate.Infrastructure.Persistence.Repositories;
using Evaluate.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;

namespace Evaluate.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var connectionString = configuration.GetConnectionString("Default") ?? "Data Source=evaluate.db";

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddScoped<DispatchDomainEventsInterceptor>();

        services.AddDbContext<EvaluateDbContext>((serviceProvider, options) =>
        {
            options.UseSqlite(connectionString);
            options.AddInterceptors(
                serviceProvider.GetRequiredService<AuditableEntitySaveChangesInterceptor>(),
                serviceProvider.GetRequiredService<DispatchDomainEventsInterceptor>());
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<EvaluateDbContext>());
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<EvaluateDbContext>());

        // Dashboard's own mock-data repositories (unrelated to the real domain below).
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IEvaluationRepository, EvaluationRepository>();
        services.AddScoped<ISubmissionRepository, SubmissionRepository>();
        services.AddScoped<IActivityFeedRepository, ActivityFeedRepository>();

        // Real domain repositories, one per aggregate. Fully qualified because the dashboard's
        // mock repositories above already claim the bare names IStudentRepository/StudentRepository
        // and IEvaluationRepository/EvaluationRepository in the usings above.
        services.AddScoped<Application.AcademicYears.IAcademicYearRepository, Repositories.AcademicYearRepository>();
        services.AddScoped<Application.Terms.ITermRepository, Repositories.TermRepository>();
        services.AddScoped<Application.Classes.IClassRepository, Repositories.ClassRepository>();
        services.AddScoped<Application.Courses.ICourseRepository, Repositories.CourseRepository>();
        services.AddScoped<Application.Topics.ITopicRepository, Repositories.TopicRepository>();
        services.AddScoped<Application.Students.IStudentRepository, Repositories.StudentRepository>();
        services.AddScoped<Application.Enrollments.IEnrollmentRepository, Repositories.EnrollmentRepository>();
        services.AddScoped<Application.TeacherCourses.ITeacherCourseRepository, Repositories.TeacherCourseRepository>();
        services.AddScoped<Application.Evaluations.IEvaluationRepository, Repositories.EvaluationRepository>();
        services.AddScoped<Application.EvaluationCriteria.IEvaluationCriteriaRepository, Repositories.EvaluationCriteriaRepository>();
        services.AddScoped<Application.AuditLogs.IAuditLogRepository, Repositories.AuditLogRepository>();

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<EvaluateDbContext>();

        services.AddScoped<IIdentityService, IdentityService>();
        services.AddSingleton<IGradingStrategy, AverageGradingStrategy>();
        services.AddScoped<IReportCardPdfGenerator, QuestPdfReportCardGenerator>();

        return services;
    }
}
