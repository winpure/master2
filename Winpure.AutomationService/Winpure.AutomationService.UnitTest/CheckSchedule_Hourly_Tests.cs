using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using WinPure.Automation.Helpers;
using WinPure.Automation.Models;
using WinPure.Automation.Services;
using WinPure.Common.Logger;
using WinPure.Configuration.Enums;
using WinPure.Configuration.Service;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Winpure.AutomationService.UnitTest
{
    [TestFixture]
    public class CheckSchedule_Hourly_Tests
    {
        private Mock<IWpLogger> _mockLogger;
        private Mock<IConfigurationService> _mockConfig;
        private Mock<IAutomationService> _mockAutomationService;
        private TestableCheckSchedule2 _scheduleChecker;
        private AutomationSchedule _hourlySchedule;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<IWpLogger>();
            _mockConfig = new Mock<IConfigurationService>();
            _mockAutomationService = new Mock<IAutomationService>();

            _scheduleChecker = new TestableCheckSchedule2(_mockLogger.Object, _mockConfig.Object);

            _hourlySchedule = new AutomationSchedule
            {
                Id = 10,
                Name = "HourlyTest",
                ScheduleType = AutomationScheduleType.Hourly,
                Frequency = 1,
                StartDate = new DateTime(2025, 5, 27),
                WeeklyTime = new DateTime(1, 1, 1, 13, 15, 0),
                ConfigurationId = 99,
                StopOnFail = false
            };

            _mockAutomationService.Setup(x => x.GetSchedules(true))
                .Returns(new List<AutomationSchedule> { _hourlySchedule });

            _mockAutomationService.Setup(x => x.GetAutomationLogs())
                .Returns(() => _scheduleChecker.ExecutedLogs.ToList());
        }

        [Test]
        public void Should_Not_Run_If_Time_Not_Reached()
        {
            var testTime = new DateTime(2025, 5, 27, 12, 45, 0);
            _scheduleChecker.CheckSchedules(_mockAutomationService.Object, testTime);
            Assert.IsFalse(_scheduleChecker.WasExecuted);
        }

        [Test]
        public void Should_Run_At_Exact_Minute_Ignoring_Seconds()
        {
            var testTime = new DateTime(2025, 5, 27, 13, 15, 0);
            _scheduleChecker.CheckSchedules(_mockAutomationService.Object, testTime);
            Assert.IsTrue(_scheduleChecker.WasExecuted);
        }


        [Test]
        public void Should_Run_At_Exact_Minute_NextIteration()
        {
            var testTime = new DateTime(2025, 5, 27, 14, 15, 0);
            _scheduleChecker.ExecutedLogs.Add(new AutomationLog()
            {
                Id = 1,
                ConfigurationId = _hourlySchedule.ConfigurationId,
                Message = AutomationHelper.AutomationConfigurationStartMessage,
                Successed = false,
                ScheduleId = _hourlySchedule.Id,
                StartDate = new DateTime(2025, 5, 27, 13, 15, 55),
                ScheduleName = _hourlySchedule.Name,
                ExecutionTime = new TimeSpan(0, 0, 0),
            });
            _scheduleChecker.ExecutedLogs.Add(new AutomationLog()
            {
                Id = 2,
                ConfigurationId = _hourlySchedule.ConfigurationId,
                Message = AutomationHelper.AutomationConfigurationFinishMessage,
                Successed = true,
                ScheduleId = _hourlySchedule.Id,
                StartDate = new DateTime(2025, 5, 27, 13, 18, 25),
                ScheduleName = _hourlySchedule.Name,
                ExecutionTime = new TimeSpan(0, 3, 0),
            });
            _hourlySchedule.StopOnFail = true;

            _scheduleChecker.CheckSchedules(_mockAutomationService.Object, testTime);
            Assert.IsTrue(_scheduleChecker.WasExecuted);
        }

        [Test]
        public void Should_Not_Run_If_Previous_Failed_And_StopOnFail_True()
        {
            _hourlySchedule.StopOnFail = true;
            _scheduleChecker.AddLog(_hourlySchedule.Id, false, new DateTime(2025, 5, 27, 13, 15, 25));
            _scheduleChecker.AddLog(_hourlySchedule.Id, false, new DateTime(2025, 5, 27, 13, 16, 35));

            var testTime = new DateTime(2025, 5, 27, 14, 15, 0);
            _scheduleChecker.CheckSchedules(_mockAutomationService.Object, testTime);
            Assert.IsFalse(_scheduleChecker.WasExecuted);
        }

        [Test]
        public void Should_Run_If_Previous_Failed_And_StopOnFail_False()
        {
            _hourlySchedule.StopOnFail = false;
            _scheduleChecker.AddLog(10, false, new DateTime(2025, 5, 27, 13, 15, 0));

            var testTime = new DateTime(2025, 5, 27, 14, 15, 0);
            _scheduleChecker.CheckSchedules(_mockAutomationService.Object, testTime);
            Assert.IsTrue(_scheduleChecker.WasExecuted);
        }

        [Test]
        public void Should_Not_Run_Immediately_After_Fail_If_StopOnFail_False()
        {
            _hourlySchedule.StopOnFail = false;
            _scheduleChecker.AddLog(10, false, new DateTime(2025, 5, 27, 13, 15, 0));

            var testTime = new DateTime(2025, 5, 27, 13, 45, 0);
            _scheduleChecker.CheckSchedules(_mockAutomationService.Object, testTime);
            Assert.IsFalse(_scheduleChecker.WasExecuted);
        }

        [Test]
        public void Should_Run_When_Previously_Blocked_By_Another_Task()
        {
            _scheduleChecker.CurrentTask = new AutomationTasks
            {
                ScheduleId = 10,
                AutomationTask = new TaskCompletionSource<bool>().Task,
                Token = new CancellationTokenSource()
            };

            var testTime1 = new DateTime(2025, 5, 27, 13, 15, 0);
            _scheduleChecker.CheckSchedules(_mockAutomationService.Object, testTime1);
            Assert.IsFalse(_scheduleChecker.WasExecuted);
            _scheduleChecker.CurrentTask = null;

            var testTime2 = new DateTime(2025, 5, 27, 13, 16, 0);
            _scheduleChecker.CheckSchedules(_mockAutomationService.Object, testTime2);
            Assert.IsTrue(_scheduleChecker.WasExecuted);
        }
    }

    internal class TestableCheckSchedule2 : CheckSchedule
    {
        public bool WasExecuted { get; private set; }
        public List<AutomationLog> ExecutedLogs { get; } = new List<AutomationLog>();

        public TestableCheckSchedule2(IWpLogger logger, IConfigurationService config)
            : base(logger, config)
        {
            
        }

        internal override void StartScheduleNow(AutomationSchedule schedule, IAutomationService automationService)
        {
            WasExecuted = true;
            AddLog(schedule.Id, true, DateTime.Now);

            CurrentTask = new AutomationTasks
            {
                ScheduleId = schedule.Id,
                AutomationTask = Task.CompletedTask,
                Token = new CancellationTokenSource()
            };
        }

        public void AddLog(int scheduleId, bool success, DateTime time)
        {
            ExecutedLogs.Add(new AutomationLog
            {
                ScheduleId = scheduleId,
                Successed = success,
                StartDate = time
            });
        }
    }
}
