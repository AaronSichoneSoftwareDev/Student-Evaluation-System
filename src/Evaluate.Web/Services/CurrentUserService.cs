using System.Security.Claims;
using Evaluate.Application.Common.Interfaces;

namespace Evaluate.Web.Services;

/// <summary>Reads the current user id from HttpContext — a presentation-layer concern,
/// which is why this lives here rather than in Infrastructure. Returns null until a
/// login flow actually populates the authenticated principal.</summary>
public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
