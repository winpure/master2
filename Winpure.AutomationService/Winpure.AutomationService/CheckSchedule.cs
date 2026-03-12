using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using WinPure.Automation.Helpers;
using WinPure.Automation.Models;
using WinPure.Automation.Services;
using Winpure.AutomationService;
using Winpure.AutomationService.DependencyInjection;
using WinPure.Common.Logger;
using WinPure.Configuration.Enums;
using WinPure.Configuration.Helper;
using WinPure.Configuration.Service;
using WinPure.Licensing.Services;
using Timer = System.Timers.Timer;

public class CheckSchedule
{
    private readonly IWpLogger _logger;
    private readonly IConfigurationService _configuration;
    private readonly object _taskLock = new object();
    private readonly Queue<AutomationSchedule> _queue = new Queue<AutomationSchedule>();
    internal AutomationTasks CurrentTask;
    private Timer _timer;
    private const int MaxQueueSize = 20;

    public CheckSchedule()
    {
        _logger = WinPureAutomationDependencyResolver.Resolve<IWpLogger>();
        _configuration = WinPureAutomationDependencyResolver.Resolve<IConfigurationService>();
        _configuration.Initiate(Program.CurrentProgramVersion);
        _logger.SetReportPath(_configuration.Configuration.LogPath);
        _logger.Information("Start Automation Service");
    }

    internal CheckSchedule(IWpLogger logger, IConfigurationService configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public void StopWatching()
    {
        _timer?.Stop();
        CurrentTask?.Token.Cancel();
    }

    public void StartWatching()
    {
        Program.WriteExtendLog("DEBUG MESSAGE: Start watching");
        var licenseService = WinPureAutomationDependencyResolver.Resolve<ILicenseService>();
        licenseService.Initiate();
        licenseService.LoadLicense(
            _configuration.Configuration.License.Path,
            _configuration.Configuration.License.AutomationLicenseName);

        _logger.Information($"Start automation v 10.2.2.0 watching Mode={licenseService.ProgramType} Demo={licenseService.IsDemo}");

        if (licenseService.IsDemo) return;

        _timer = new Timer(60 * 1000);
        _timer.Elapsed += _timer_Elapsed;
        _timer.Start();
    }

    private void _timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        _timer.Stop();

        try
        {
            if (!_configuration.Configuration.EnableAutomation) return;

            var automationService = WinPureAutomationDependencyResolver.Resolve<IAutomationService>();
            var nowRounded = DateTime.Now.TrimSeconds();
            CheckSchedules(automationService, nowRounded);
        }
        catch (Exception ex)
        {
            _logger.Debug("Automation schedule ERROR", ex.Message, ex.StackTrace);
            Program.WriteExtendLog($"Automation schedule ERROR Message: {ex.Message}\nStackTrace: {ex.StackTrace}", ex);
        }
        finally
        {
            _timer.Start();
        }
    }

    internal void CheckSchedules(IAutomationService automationService, DateTime nowRounded)
    {
        _logger.Information($"Run timer Time={DateTime.Now}");

        var logs = automationService.GetAutomationLogs();
        var schedules = automationService.GetSchedules(true);

        foreach (var schedule in schedules)
        {
            if (schedule.StopOnFail)
            {
                var lastLog = logs
                    .Where(x => x.ScheduleId == schedule.Id && x.Message != AutomationHelper.AutomationConfigurationStartMessage)
                    .OrderByDescending(x => x.StartDate)
                    .FirstOrDefault();

                if (lastLog != null && !lastLog.Successed)
                    continue;
            }

            var expectedTime = schedule.StartDate.Date.Add(schedule.WeeklyTime.TimeOfDay).TrimSeconds();
            bool shouldRun = false;

            switch (schedule.ScheduleType)
            {
                case AutomationScheduleType.Once:
                    shouldRun = nowRounded == expectedTime && !logs.Any(x => x.ScheduleId == schedule.Id);
                    break;

                case AutomationScheduleType.Hourly:
                    var freqH = schedule.Frequency ?? 1;
                    var diffH = (nowRounded - expectedTime).TotalMinutes;
                    shouldRun = diffH >= 0 && diffH % (freqH * 60) == 0 && !logs.Any(x => x.ScheduleId == schedule.Id && x.StartDate == nowRounded);
                    break;

                case AutomationScheduleType.Daily:
                    var freqD = schedule.Frequency ?? 1;
                    var daysPassed = (nowRounded.Date - expectedTime.Date).Days;
                    shouldRun = daysPassed >= 0 && daysPassed % freqD == 0 && nowRounded.TimeOfDay == expectedTime.TimeOfDay && !logs.Any(x => x.ScheduleId == schedule.Id && x.StartDate == nowRounded);
                    break;

                case AutomationScheduleType.Weekly:
                    var days = schedule.DayOfWeek?.ToLower().Split(' ') ?? Array.Empty<string>();
                    var currentPrefix = nowRounded.DayOfWeek.ToString().Substring(0, 2).ToLower();
                    shouldRun = days.Contains(currentPrefix) && nowRounded.TimeOfDay == expectedTime.TimeOfDay && !logs.Any(x => x.ScheduleId == schedule.Id && x.StartDate == nowRounded);
                    break;

                case AutomationScheduleType.Monthly:
                    var day = schedule.DayOfMonth ?? 1;
                    if (DateTime.DaysInMonth(nowRounded.Year, nowRounded.Month) >= day && nowRounded.Day == day)
                    {
                        shouldRun = nowRounded.TimeOfDay == expectedTime.TimeOfDay && !logs.Any(x => x.ScheduleId == schedule.Id && x.StartDate == nowRounded);
                    }
                    break;
            }

            if (shouldRun)
            {
                RunScheduleAsync(schedule, automationService);
            }
        }

        lock (_taskLock)
        {
            if ((CurrentTask == null || CurrentTask.AutomationTask.IsCompleted) && _queue.Count > 0)
            {
                var next = _queue.Dequeue();
                StartScheduleNow(next, automationService);
            }
        }
    }

    private void RunScheduleAsync(AutomationSchedule schedule, IAutomationService automationService)
    {
        lock (_taskLock)
        {
            if (CurrentTask == null || CurrentTask.AutomationTask.IsCompleted)
            {
                StartScheduleNow(schedule, automationService);
            }
            else
            {
                if (_queue.Any(x => x.Id == schedule.Id)) return;
                if (_queue.Count >= MaxQueueSize) return;
                _queue.Enqueue(schedule);
            }
        }
    }

    internal virtual void StartScheduleNow(AutomationSchedule schedule, IAutomationService automationService)
    {
        var cts = new CancellationTokenSource();

        var task = new Task(() =>
        {
            RunConfiguration(schedule.ConfigurationId, schedule.Id, cts, automationService);

            lock (_taskLock)
            {
                CurrentTask = null;

                if (_queue.Count > 0)
                {
                    var next = _queue.Dequeue();
                    StartScheduleNow(next, automationService);
                }
            }
        });

        CurrentTask = new AutomationTasks
        {
            AutomationTask = task,
            Token = cts,
            ScheduleId = schedule.Id
        };

        task.Start();
    }

    private void RunConfiguration(int configurationId, int scheduleId, CancellationTokenSource cancellationTokenSource, IAutomationService automationService)
    {
        Program.WriteExtendLog($"DEBUG MESSAGE: Execute schedule {scheduleId} for automation {configurationId}");
        var log = new AutomationLog
        {
            Message = AutomationHelper.AutomationConfigurationFinishMessage,
            Successed = true,
            StartDate = DateTime.UtcNow,
            ScheduleId = scheduleId
        };

        var start = DateTime.UtcNow;

        try
        {
            automationService?.AddAutomationLog(new AutomationLog
            {
                Message = AutomationHelper.AutomationConfigurationStartMessage,
                Successed = false,
                StartDate = start,
                ScheduleId = scheduleId
            });

            var cfg = automationService.GetAutomation(configurationId);
            _logger.Information($"AUTOMATION Configuration {cfg.Name} starting");
            var result = AutomationExecutor.RunAutomation(cfg, cancellationTokenSource);

            if (!string.IsNullOrEmpty(result))
            {
                log.Message += " " + result;
                log.Successed = false;
            }
        }
        catch (Exception ex)
        {
            Program.WriteExtendLog("RUN CONFIGURATION ERROR", ex);
            _logger.Debug("AUTOMATION ERROR", ex.Message, ex.StackTrace);
            log.Message = $"ERROR! {ex.Message}";
            log.Successed = false;
        }
        finally
        {
            log.ExecutionTime = DateTime.UtcNow - start;
            automationService?.AddAutomationLog(log);
        }
    }
}
