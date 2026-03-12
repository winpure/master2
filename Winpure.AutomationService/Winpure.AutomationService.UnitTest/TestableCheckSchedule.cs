using System.Threading.Tasks;
using System.Threading;
using WinPure.Automation.Models;
using WinPure.Automation.Services;
using WinPure.Common.Logger;
using WinPure.Configuration.Service;

namespace Winpure.AutomationService.UnitTest
{
    internal class TestableCheckSchedule : CheckSchedule
    {
        public bool WasExecuted { get; private set; }

        public TestableCheckSchedule(IWpLogger logger, IConfigurationService config)
            : base(logger, config)
        {
        }

        internal override void StartScheduleNow(AutomationSchedule schedule, IAutomationService automationService)
        {
            WasExecuted = true;

            // Optional: simulate completion
            CurrentTask = new AutomationTasks
            {
                ScheduleId = schedule.Id,
                AutomationTask = Task.CompletedTask,
                Token = new CancellationTokenSource()
            };
        }
    }
}