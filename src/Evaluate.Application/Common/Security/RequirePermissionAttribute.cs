namespace Evaluate.Application.Common.Security;

/// <summary>Marks a command/query as requiring a specific permission claim. Read by
/// <c>AuthorizationBehaviour</c>, which throws <see cref="Exceptions.ForbiddenAccessException"/>
/// when the current user lacks it.</summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class RequirePermissionAttribute(string permission) : Attribute
{
    public string Permission { get; } = permission;
}
