namespace Evaluate.Web.Components.Shared;

/// <summary>One filter tab on a <see cref="DataTablePanel{TItem}"/> — "All" (Predicate
/// null) plus one tab per meaningful status/category value for that table.</summary>
public record TableTab<TItem>(string Key, string Label, Func<TItem, bool>? Predicate = null);
