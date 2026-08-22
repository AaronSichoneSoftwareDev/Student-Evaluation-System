using Evaluate.Application.Common.Interfaces;

namespace Evaluate.Application.Tests.Common;

/// <summary>Mirrors production's <c>CurrentUserService</c> when no one is logged in —
/// UserId is always null, exactly like the real app until a login UI exists.</summary>
public class FakeCurrentUserService : ICurrentUserService
{
    public string? UserId => null;
}
