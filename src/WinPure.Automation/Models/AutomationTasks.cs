using System.Threading;
using System.Threading.Tasks;

namespace WinPure.Automation.Models;

internal class AutomationTasks
{
    public int ScheduleId { get; set; }
    public Task AutomationTask { get; set; }
    public CancellationTokenSource Token { get; set; }
}