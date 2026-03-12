using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using WinPure.Automation.Models;
using WinPure.Automation.Services;
using WinPure.Configuration.Enums;
using WinPure.Configuration.Service;
using WinPure.Common.Logger;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Winpure.AutomationService.UnitTest
{
    [TestFixture]
    internal class CheckSchedule_Daily_Tests
    {
        private Mock<IWpLogger> _mockLogger;
        private Mock<IConfigurationService> _mockConfig;
        private Mock<IAutomationService> _mockAutomationService;
        private TestableCheckSchedule _scheduleChecker;
        private AutomationSchedule _dailySchedule;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<IWpLogger>();
            _mockConfig = new Mock<IConfigurationService>();
            _mockAutomationService = new Mock<IAutomationService>();

            _scheduleChecker = new TestableCheckSchedule(_mockLogger.Object, _mockConfig.Object);

            _dailySchedule = new AutomationSchedule
            {
                Id = 70,
                Name = "Order_Queue_Import_Automation_Daily_A",
                ScheduleType = AutomationScheduleType.Daily,
                Frequency = 1,
                StartDate = new DateTime(2025, 5, 21, 5, 0, 0), // Local time
                WeeklyTime = new DateTime(1, 1, 1, 7, 30, 0),   // 07:30 daily
                ConfigurationId = 12,
                StopOnFail = true
            };

            _mockAutomationService.Setup(x => x.GetSchedules(true))
                .Returns(new List<AutomationSchedule> { _dailySchedule });
        }

        [Test]
        public void Should_Not_Run_If_Time_Not_Reached_Yet()
        {
            var testTime = new DateTime(2025, 5, 21, 7, 15, 0); // before 07:30
            _mockAutomationService.Setup(x => x.GetAutomationLogs())
                .Returns(new List<AutomationLog>());

            _scheduleChecker.CheckSchedules(_mockAutomationService.Object, testTime);

            Assert.IsFalse(_scheduleChecker.WasExecuted, "Task should not run before scheduled time.");
        }

        [Test]
        public void Should_Run_At_Exact_Scheduled_Time_When_No_Logs()
        {
            var testTime = new DateTime(2025, 5, 21, 7, 30, 0);
            _mockAutomationService.Setup(x => x.GetAutomationLogs())
                .Returns(new List<AutomationLog>());

            _scheduleChecker.CheckSchedules(_mockAutomationService.Object, testTime);

            Assert.IsTrue(_scheduleChecker.WasExecuted, "Task should run at exact scheduled time.");
        }

        [Test]
        public void Should_Not_Run_If_Previous_Failed_And_StopOnFail()
        {
            var testTime = new DateTime(2025, 5, 21, 7, 35, 0);
            _mockAutomationService.Setup(x => x.GetAutomationLogs())
                .Returns(new List<AutomationLog>
                {
                    new AutomationLog
                    {
                        ScheduleId = 70,
                        Successed = false,
                        StartDate = new DateTime(2025, 5, 21, 7, 30, 0)
                    }
                });

            _scheduleChecker.CheckSchedules(_mockAutomationService.Object, testTime);

            Assert.IsFalse(_scheduleChecker.WasExecuted, "Task should not rerun after failure if StopOnFail is true.");
        }

        [TestCase(7, 35)]
        [TestCase(9, 0)]
        public void Should_Not_Run_Again_In_Same_Day_After_Success(int hour, int minute)
        {
            var testTime = new DateTime(2025, 5, 21, hour, minute, 0);
            _mockAutomationService.Setup(x => x.GetAutomationLogs())
                .Returns(new List<AutomationLog>
                {
                    new AutomationLog
                    {
                        ScheduleId = 70,
                        Successed = true,
                        StartDate = new DateTime(2025, 5, 21, 7, 30, 0)
                    }
                });

            _scheduleChecker.CheckSchedules(_mockAutomationService.Object, testTime);

            Assert.IsFalse(_scheduleChecker.WasExecuted, $"Task should not run again on same day at {hour}:{minute:00}.");
        }

        [Test]
        public void Should_Run_Next_Day_After_Success()
        {
            var testTime = new DateTime(2025, 5, 22, 7, 30, 0);
            _mockAutomationService.Setup(x => x.GetAutomationLogs())
                .Returns(new List<AutomationLog>
                {
                    new AutomationLog
                    {
                        ScheduleId = 70,
                        Successed = true,
                        StartDate = new DateTime(2025, 5, 21, 7, 30, 0)
                    }
                });

            _scheduleChecker.CheckSchedules(_mockAutomationService.Object, testTime);

            Assert.IsTrue(_scheduleChecker.WasExecuted, "Task should run on the next day after a successful run.");
        }

        [Test]
        public void Should_Not_Run_Next_Day_If_Failed_And_StopOnFail()
        {
            var testTime = new DateTime(2025, 5, 22, 7, 30, 0);
            _mockAutomationService.Setup(x => x.GetAutomationLogs())
                .Returns(new List<AutomationLog>
                {
                    new AutomationLog
                    {
                        ScheduleId = 70,
                        Successed = false,
                        StartDate = new DateTime(2025, 5, 21, 7, 30, 0)
                    }
                });

            _scheduleChecker.CheckSchedules(_mockAutomationService.Object, testTime);

            Assert.IsFalse(_scheduleChecker.WasExecuted, "Task should not run again the next day if the previous run failed and StopOnFail is true.");
        }
    }
}
