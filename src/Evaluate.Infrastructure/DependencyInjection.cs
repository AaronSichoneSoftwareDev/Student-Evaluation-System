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

        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IEvaluationRepository, EvaluationRepository>();
        services.AddScoped<ISubmissionRepository, SubmissionRepository>();
        services.AddScoped<IActivityFeedRepository, ActivityFeedRepository>();
        services.AddScoped<IInboxRepository, InboxRepository>();
        services.AddScoped<IInstructorRepository, InstructorRepository>();

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
