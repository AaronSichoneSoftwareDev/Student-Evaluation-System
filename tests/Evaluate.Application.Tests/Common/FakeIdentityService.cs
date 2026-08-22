using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using Evaluate.Application.Identity;

namespace Evaluate.Application.Tests.Common;

/// <summary>Not used by any test scenario yet — <see cref="AuthorizationBehaviour{TRequest,TResponse}"/>
/// only calls into this when a user id is present, and <see cref="FakeCurrentUserService"/> never
/// supplies one. Exists purely to satisfy DI registration for the pipeline test.</summary>
public class FakeIdentityService : IIdentityService
{
    public Task<string?> GetUserNameAsync(string userId, CancellationToken cancellationToken = default) => Task.FromResult<string?>("Test User");

    public Task<List<UserSummary>> GetUsersAsync(CancellationToken cancellationToken = default) => Task.FromResult(new List<UserSummary>());

    public Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellationToken = default) => Task.FromResult(false);

    public Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellationToken = default) => Task.FromResult(false);

    public Task<(Result Result, string UserId)> CreateUserAsync(string userName, string email, string password, string firstName, string lastName, CancellationToken cancellationToken = default) =>
        Task.FromResult((Result.Success(), "fake-user-id"));

    public Task<Result> AddToRoleAsync(string userId, string role, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success());

    public Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken = default) => Task.FromResult(Result.Success());
}
