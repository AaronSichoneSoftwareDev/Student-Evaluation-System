namespace Evaluate.Application.Identity;

public record UserSummary(string Id, string UserName, string? Email, string FirstName, string LastName, bool IsActive, IReadOnlyList<string> Roles);
