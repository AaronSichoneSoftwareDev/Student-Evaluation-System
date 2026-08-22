using Evaluate.Domain.Entities.Dashboard;

namespace Evaluate.Application.Common.Interfaces;

public interface IInboxRepository
{
    Task<IReadOnlyList<InboxMessage>> GetAllAsync(CancellationToken cancellationToken = default);
}
