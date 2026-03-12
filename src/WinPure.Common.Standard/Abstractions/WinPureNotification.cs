namespace WinPure.Common.Abstractions;

internal class WinPureNotification : IWinPureNotification
{
    public event Action<string, Exception> OnException;
    public event Action<string, int> OnProgressUpdate;
    public void NotifyException(string message, Exception ex)
    {
        OnException?.Invoke(message, ex);
    }

    public void NotifyProgress(string message, int progress)
    {
        OnProgressUpdate?.Invoke(message, progress);
    }
}