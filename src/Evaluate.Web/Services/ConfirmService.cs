namespace Evaluate.Web.Services;

public enum ConfirmVariant { Danger, Warning, Info }

public record ConfirmRequest(string Title, string Message, string ConfirmText, string CancelText, ConfirmVariant Variant);

/// <summary>Scoped, promise-style replacement for the browser's native <c>confirm()</c> —
/// <c>await ConfirmService.ConfirmAsync(...)</c> shows <c>ConfirmDialogHost</c> (mounted once
/// in the layout) and resolves once the user picks Cancel or Confirm.</summary>
public class ConfirmService
{
    private TaskCompletionSource<bool>? _pending;

    public event Action<ConfirmRequest>? OnShow;

    public Task<bool> ConfirmAsync(
        string title,
        string message,
        string confirmText = "Delete",
        string cancelText = "Cancel",
        ConfirmVariant variant = ConfirmVariant.Danger)
    {
        // A previous, unresolved confirmation shouldn't be left hanging if a new one is
        // requested — resolve it as "cancelled" rather than leaking the awaiting caller.
        _pending?.TrySetResult(false);

        _pending = new TaskCompletionSource<bool>();
        OnShow?.Invoke(new ConfirmRequest(title, message, confirmText, cancelText, variant));
        return _pending.Task;
    }

    public void Resolve(bool confirmed) => _pending?.TrySetResult(confirmed);
}
