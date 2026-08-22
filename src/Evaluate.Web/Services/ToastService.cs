namespace Evaluate.Web.Services;

public enum ToastVariant { Success, Error, Warning, Info }

public record ToastMessage(Guid Id, string Title, string Message, ToastVariant Variant);

/// <summary>Scoped (one per Blazor circuit / user) so toasts stay private to whoever
/// triggered them. <see cref="OnChange"/> is how <c>ToastContainer</c> knows to re-render;
/// it may fire from a timer thread, so subscribers must marshal back via InvokeAsync.</summary>
public class ToastService
{
    private const int AutoDismissMilliseconds = 5000;

    private readonly List<ToastMessage> _toasts = [];

    public IReadOnlyList<ToastMessage> Toasts => _toasts;

    public event Action? OnChange;

    public void ShowSuccess(string title, string message) => Show(title, message, ToastVariant.Success);

    public void ShowError(string title, string message) => Show(title, message, ToastVariant.Error);

    public void ShowWarning(string title, string message) => Show(title, message, ToastVariant.Warning);

    public void ShowInfo(string title, string message) => Show(title, message, ToastVariant.Info);

    private void Show(string title, string message, ToastVariant variant)
    {
        var toast = new ToastMessage(Guid.NewGuid(), title, message, variant);
        _toasts.Add(toast);
        OnChange?.Invoke();

        _ = Task.Delay(AutoDismissMilliseconds).ContinueWith(_ => Remove(toast.Id), TaskScheduler.Default);
    }

    public void Remove(Guid id)
    {
        if (_toasts.RemoveAll(t => t.Id == id) > 0)
        {
            OnChange?.Invoke();
        }
    }
}
