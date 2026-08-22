namespace Evaluate.Application.Common.Interfaces;

/// <summary>Commits whatever changes were staged against repositories in this request. Kept
/// separate from the repository interfaces themselves — a repository's job is exposing a
/// collection-like view of one aggregate, not deciding when to persist.</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
