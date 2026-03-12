using System.Threading;
using System.Threading.Tasks;

namespace WinPure.Common.Abstractions;

internal interface IWpServiceBase
{
    event Action<string, Task, bool, CancellationTokenSource> OnProgressShow;
    event Action<string, int> OnProgressUpdate;
    event Action<string, string, MessagesType, Exception> OnException;
}