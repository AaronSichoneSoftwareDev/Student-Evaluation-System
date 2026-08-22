using Evaluate.Application.Common.Behaviours;
using Evaluate.Application.Dashboard;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Evaluate.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IDashboardService, DashboardService>();

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            config.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            config.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
            config.AddOpenBehavior(typeof(ValidationBehaviour<,>));
            config.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
        });

        return services;
    }
}
