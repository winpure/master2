using Microsoft.EntityFrameworkCore;
using System.Linq;
using WinPure.Automation.Enums;
using WinPure.Configuration.Helper;
using WinPure.Configuration.Infrastructure;
using WinPure.Configuration.Models.Database;

namespace WinPure.Automation.DependencyInjection;

internal static partial class WinPureAutomationDependencyExtension
{
    private class AutomationRepository : IAutomationRepository
    {
        private readonly WinPureConfigurationContext _configurationContext;

        public AutomationRepository(WinPureConfigurationContext configurationContext)
        {
            _configurationContext = configurationContext;
        }

        public void SaveChanges()
        {
            _configurationContext.SaveChanges();
        }

        public List<AutomationHeader> GetAutomation()
        {
            return _configurationContext.AutomationHeaders
                .OrderByDescending(x => x.Id)
                .Select(x => new AutomationHeader
                {
                    Id = x.Id,
                    Name = x.ConfigurationName,
                    IsActive = x.IsActive,
                    CreationDate = x.CreationDate.ToLocalTimeSafe()
                }).ToList();
        }

        public AutomationHeaderEntity GetAutomationEntity(string name)
        {
            return _configurationContext.AutomationHeaders
                .FirstOrDefault(x => x.ConfigurationName == name);
        }

        public AutomationHeaderEntity GetAutomationEntity(int id)
        {
            return _configurationContext.AutomationHeaders
                .Include(x => x.ConfigurationSteps)
                .FirstOrDefault(x => x.Id == id);
        }

        public AutomationHeaderEntity GetAutomationEntityWithAllDetails(int id)
        {
            return _configurationContext.AutomationHeaders
                .Include(i => i.ConfigurationSteps)
                .Include(i => i.Schedules)
                .FirstOrDefault(x => x.Id == id);
        }

        public void AddConfiguration(AutomationHeaderEntity automationHeader)
        {
            _configurationContext.AutomationHeaders.Add(automationHeader);
        }

        public void RemoveConfigurationHeader(AutomationHeaderEntity automationHeader)
        {
            _configurationContext.AutomationHeaders.Remove(automationHeader);
        }

        public AutomationConfiguration GetConfiguration(int id)
        {
            var configuration = _configurationContext.AutomationHeaders.AsNoTracking()
                .Include(i => i.ConfigurationSteps)
                .Include(i => i.Schedules)
                .Where(x => x.Id == id)
                .Select(x => new AutomationConfiguration
                {
                    Id = x.Id,
                    Name = x.ConfigurationName,
                    IsActive = x.IsActive,
                    CreationDate = x.CreationDate.ToLocalTimeSafe(),
                }).FirstOrDefault();

            if (configuration != null)
            {
                configuration.Steps = _configurationContext.AutomationConfigurationSteps
                    .Where(x => x.ConfigurationId == id)
                    .OrderBy(x => x.Id)
                    .Select(x => new AutomationStep
                    {
                        Id = x.Id,
                        ConfigurationId = x.ConfigurationId,
                        SourceName = x.SourceName,
                        Param1 = x.Parameter1,
                        Param2 = x.Parameter2,
                        StepType = (AutomationStepType)x.StepId,
                    }).ToList();

                for (int i = 0; i < configuration.Steps.Count; i++)
                {
                    configuration.Steps[i].Order = i;
                }
            }

            return configuration;
        }


        public List<AutomationSchedule> GetSchedules(bool onlyActive = false)
        {
            var qry = _configurationContext.AutomationSchedules.AsNoTracking()
                .Include(x => x.Configuration)
                .AsQueryable();

            if (onlyActive)
            {
                qry = qry.Where(x => x.IsActive && x.Configuration.IsActive);
            }

            return qry.Select(x => new AutomationSchedule
            {
                Id = x.Id,
                ConfigurationId = x.ConfigurationId,
                ConfigurationName = x.Configuration.ConfigurationName,
                Frequency = x.Frequency,
                Name = x.ScheduleName,
                StartDate = x.StartDate.ToLocalTimeSafe(),
                ScheduleType = x.ScheduleType,
                DayOfMonth = x.DayOfMonth,
                DayOfWeek = x.DayOfWeek,
                IsActive = x.IsActive,
                StopOnFail = x.StopOnFail,
                WeeklyTime = x.WeeklyTime
            }).ToList();
        }

        public List<AutomationSchedule> GetSchedulesForConfiguration(int configurationId)
        {
            return _configurationContext.AutomationSchedules.AsNoTracking()
                .Include(x => x.Configuration)
                .Where(x => x.ConfigurationId == configurationId)
                .OrderBy(x => x.Id)
                .Select(x => new AutomationSchedule
                {
                    Id = x.Id,
                    ConfigurationId = x.ConfigurationId,
                    ConfigurationName = x.Configuration.ConfigurationName,
                    Frequency = x.Frequency,
                    Name = x.ScheduleName,
                    StartDate = x.StartDate.ToLocalTimeSafe(),
                    ScheduleType = x.ScheduleType,
                    DayOfMonth = x.DayOfMonth,
                    DayOfWeek = x.DayOfWeek,
                    IsActive = x.IsActive,
                    StopOnFail = x.StopOnFail,
                    WeeklyTime = x.WeeklyTime
                }).ToList();
        }

        public AutomationScheduleEntity GetScheduleEntity(int id)
        {
            return _configurationContext.AutomationSchedules.FirstOrDefault(x => x.Id == id);
        }

        public void AddSchedule(AutomationScheduleEntity automationSchedule)
        {
            _configurationContext.AutomationSchedules.Add(automationSchedule);
        }

        public void RemoveSchedule(int id)
        {
            var schedule = _configurationContext.AutomationSchedules.FirstOrDefault(x => x.Id == id);
            if (schedule != null)
            {
                _configurationContext.AutomationSchedules.Remove(schedule);
                _configurationContext.SaveChanges();
            }
        }

        public void RemoveSchedule(AutomationScheduleEntity scheduleEntity)
        {
            _configurationContext.AutomationSchedules.Remove(scheduleEntity);
        }

        public void RemoveConfigurationStep(AutomationConfigurationStepEntity automationConfigurationStep)
        {
            _configurationContext.AutomationConfigurationSteps.Remove(automationConfigurationStep);
        }


        public void RemoveLog(AutomationLogEntity automationLog)
        {
            _configurationContext.AutomationLogs.Remove(automationLog);
        }

        public List<AutomationLog> GetLogs()
        {
            return _configurationContext.AutomationLogs.AsNoTracking()
                .Include(i => i.Schedule)
                .Include(i => i.Schedule.Configuration)
                .OrderByDescending(x => x.Id)
                .Select(x => new AutomationLog
                {
                    Id = x.Id,
                    ScheduleId = x.ScheduleId,
                    ScheduleName = x.Schedule.ScheduleName,
                    ConfigurationId = x.Schedule.ConfigurationId,
                    ConfigurationName = x.Schedule.Configuration.ConfigurationName,
                    Message = x.Message,
                    StartDate = x.DateOfRun.ToLocalTimeSafe(),
                    Successed = x.Successful,
                    ExecutionTime = x.ExecutionTime
                }).ToList();
        }

        public void AddLog(AutomationLogEntity automationLogEntity)
        {
            _configurationContext.AutomationLogs.Add(automationLogEntity);
        }
    }
}