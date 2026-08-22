using Evaluate.Domain.Entities.Dashboard;

namespace Evaluate.Application.Common.Interfaces;

public interface IInstructorRepository
{
    Task<Instructor?> GetFeaturedAsync(CancellationToken cancellationToken = default);
}
