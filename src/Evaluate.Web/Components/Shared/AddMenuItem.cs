using Microsoft.AspNetCore.Components;

namespace Evaluate.Web.Components.Shared;

/// <summary>One option in a <see cref="PageHeader"/>'s "+ Add New" dropdown, for pages that
/// manage more than one entity type (e.g. Academic Setup: Year / Term / Class).</summary>
public record AddMenuItem(string Label, EventCallback OnClick);
