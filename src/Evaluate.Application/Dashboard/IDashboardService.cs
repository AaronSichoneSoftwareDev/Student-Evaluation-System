namespace Evaluate.Application.Dashboard;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default);
}
