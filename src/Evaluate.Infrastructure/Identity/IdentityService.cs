using Evaluate.Application.Common.Interfaces;
using Evaluate.Application.Common.Models;
using Evaluate.Application.Common.Security;
using Evaluate.Application.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Evaluate.Infrastructure.Identity;

public class IdentityService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager) : IIdentityService
{
    public async Task<string?> GetUserNameAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user is null ? null : $"{user.FirstName} {user.LastName}";
    }

    public async Task<List<UserSummary>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var summaries = new List<UserSummary>();

        foreach (var user in userManager.Users.ToList())
        {
            var roles = await userManager.GetRolesAsync(user);
            summaries.Add(new UserSummary(user.Id, user.UserName ?? string.Empty, user.Email, user.FirstName, user.LastName, user.IsActive, roles.ToList()));
        }

        return summaries;
    }

    public async Task<List<UserSummary>> GetUsersByIdsAsync(IReadOnlyList<string> userIds, CancellationToken cancellationToken = default)
    {
        if (userIds.Count == 0)
        {
            return [];
        }

        var users = await userManager.Users.Where(u => userIds.Contains(u.Id)).ToListAsync(cancellationToken);

        var summaries = new List<UserSummary>(users.Count);
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            summaries.Add(new UserSummary(user.Id, user.UserName ?? string.Empty, user.Email, user.FirstName, user.LastName, user.IsActive, roles.ToList()));
        }

        return summaries;
    }

    public async Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user is not null && await userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return false;
        }

        var roleNames = await userManager.GetRolesAsync(user);

        foreach (var roleName in roleNames)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
            {
                continue;
            }

            var claims = await roleManager.GetClaimsAsync(role);
            if (claims.Any(c => c.Type == Permissions.ClaimType && c.Value == policyName))
            {
                return true;
            }
        }

        return false;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string email, string password, string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
        };

        var result = await userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<Result> AddToRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Result.Failure("User not found.");
        }

        var result = await userManager.AddToRoleAsync(user, role);
        return result.ToApplicationResult();
    }

    public async Task<Result> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        return user is null ? Result.Success() : (await userManager.DeleteAsync(user)).ToApplicationResult();
    }
}
