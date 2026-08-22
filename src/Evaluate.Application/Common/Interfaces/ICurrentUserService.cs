namespace Evaluate.Application.Common.Interfaces;

/// <summary>Implemented in the Web project (it needs HttpContext, a presentation-layer
/// concern) — Application only depends on this abstraction.</summary>
public interface ICurrentUserService
{
    string? UserId { get; }
}
