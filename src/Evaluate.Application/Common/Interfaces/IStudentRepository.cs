using Evaluate.Domain.Entities.Dashboard;

namespace Evaluate.Application.Common.Interfaces;

public interface IStudentRepository
{
    Task<IReadOnlyList<Student>> GetAllAsync(CancellationToken cancellationToken = default);
}
