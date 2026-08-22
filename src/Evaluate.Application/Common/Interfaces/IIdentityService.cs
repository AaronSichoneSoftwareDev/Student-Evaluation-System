using Evaluate.Application.Common.Models;
using Evaluate.Application.Identity;

namespace Evaluate.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId, CancellationToken cancellationToken = default);

    Task<List<UserSummary>> GetUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>Resolves only the given user ids — a single filtered query, not the full user table,
    /// for call sites (like the class -> teacher dropdown) that already know exactly which users they need.</summary>
    Task<List<UserSummary>> GetUsersByIdsAsync(IReadOnlyList<string> userIds, CancellationToken cancellationToken = default);

    Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellationToken = default);

    Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellationToken = default);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string email, string password, string firstName, string lastName, CancellationToken cancellationToken = default);

    Task<Result> AddToRoleAsync(string userId, string role, CancellationToken cancellationToken = default);

    Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
}
