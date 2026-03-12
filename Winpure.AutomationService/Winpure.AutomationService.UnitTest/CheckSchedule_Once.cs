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
    public class CheckSchedule_Once
    {
        private Mock<IWpLogger> _mockLogger;
        private Mock<IConfigurationService> _mockConfig;
        private Mock<IAutomationService> _mockAutomationService;
        private TestableCheckSchedule _scheduleChecker;
        private AutomationSchedule _onceSchedule;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<IWpLogger>();
            _mockConfig = new Mock<IConfigurationService>();
            _mockAutomationService = new Mock<IAutomationService>();

            _scheduleChecker = new TestableCheckSchedule(_mockLogger.Object, _mockConfig.Object);

            _onceSchedule = new AutomationSchedule
            {
                Id = 1,
                Name = "TestOnce",
                ScheduleType = AutomationScheduleType.Once,
                StartDate = new DateTime(2025, 5, 1),
                WeeklyTime = new DateTime(1, 1, 1, 14, 10, 53),
                ConfigurationId = 123
            };

            _mockAutomationService.Setup(x => x.GetSchedules(true))
                .Returns(new List<AutomationSchedule> { _onceSchedule });
        }

        [Test]
        public void Schedule_ShouldNotRun_IfTimeIsBeforeStart()
        {
            var testTime = new DateTime(2025, 5, 1, 13, 15, 0); // до времени запуска
            _mockAutomationService.Setup(x => x.GetAutomationLogs())
                .Returns(new List<AutomationLog>());

            _scheduleChecker.CheckSchedules(_mockAutomationService.Object, testTime);

            Assert.IsFalse(_scheduleChecker.WasExecuted, "Task should not run before scheduled time.");
        }

        [Test]
        public void Schedule_ShouldRun_AtExactScheduledTime()
        {
            var testTime = new DateTime(2025, 5, 1, 14, 10, 0);
            _mockAutomationService.Setup(x => x.GetAutomationLogs())
                .Returns(new List<AutomationLog>());

            _scheduleChecker.CheckSchedules(_mockAutomationService.Object, testTime);

            Assert.IsTrue(_scheduleChecker.WasExecuted, "Task should run at exact time.");
        }

        [Test]
        public void Schedule_ShouldNotRun_IfAlreadyExecuted()
        {
            var testTime = new DateTime(2025, 5, 2, 10, 30, 0);
            _mockAutomationService.Setup(x => x.GetAutomationLogs())
                .Returns(new List<AutomationLog>
                {
                    new AutomationLog
                    {
                        ScheduleId = 1,
                        Successed = true,
                        StartDate = new DateTime(2025, 5, 1, 14, 10, 0)
                    }
                });

            _scheduleChecker.CheckSchedules(_mockAutomationService.Object, testTime);

            Assert.IsFalse(_scheduleChecker.WasExecuted, "Task should not run second time");
        }
    }
}
