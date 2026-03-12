namespace WinPure.Common.Abstractions;

internal interface IWinPureNotification
{
    event Action<string, Exception> OnException;
    event Action<string, int> OnProgressUpdate;
}