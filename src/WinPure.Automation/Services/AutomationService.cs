using System.Threading;
using System.Threading.Tasks;
using WinPure.Common.Enums;

namespace WinPure.Automation.DependencyInjection;

internal static partial class WinPureAutomationDependencyExtension
{
    private class AutomationService : IAutomationService
    {
        private readonly IAutomationRepository _automationRepository;
        private readonly IWpLogger _logger;

        public event Action<string, Task, bool, CancellationTokenSource> OnProgressShow;
        public event Action<string, int> OnProgressUpdate;
        public event Action<string, string, MessagesType, Exception> OnException;

        public event Action OnAutomationListUpdated;

        public AutomationService(IAutomationRepository automationRepository, IWpLogger logger)
        {
            _automationRepository = automationRepository;
            _logger = logger;
        }

        public bool IsNameExists(string name)
        {
            var automation = _automationRepository.GetAutomationEntity(name);
            return automation != null;
        }

        public int AddAutomation(AutomationConfiguration configuration)
        {
            try
            {
                var entity = configuration.ToEntity();
                _automationRepository.AddConfiguration(entity);
                _automationRepository.SaveChanges();
                OnAutomationListUpdated?.Invoke();

                return entity.Id;
            }
            catch (Exception e)
            {
                _logger.Error("Cannot create automation", e);
                OnException?.Invoke("Cannot add automation entity", "", MessagesType.Error, e);
            }

            return -1;
        }

        public void UpdateAutomation(AutomationConfiguration configuration)
        {
            try
            {
                var existingAutomation = _automationRepository.GetAutomationEntity(configuration.Id);
                existingAutomation.ConfigurationName = configuration.Name;
                existingAutomation.ConfigurationSteps.Clear();
                existingAutomation.ConfigurationSteps = configuration.ToEntity().ConfigurationSteps;
                _automationRepository.SaveChanges();

                OnAutomationListUpdated?.Invoke();
            }
            catch (Exception e)
            {
                _logger.Error("Cannot update automation", e);
                OnException?.Invoke("Cannot update automation entity", "", MessagesType.Error, e);
            }
        }

        public int AddAutomationSchedule(AutomationSchedule schedule)
        {
            try
            {
                var entity = schedule.ToEntity();
                _automationRepository.AddSchedule(entity);
                _automationRepository.SaveChanges();
                return entity.Id;
            }
            catch (Exception e)
            {
                _logger.Error("Cannot add schedule to automation", e);
                OnException?.Invoke("Cannot add automation schedule", "", MessagesType.Error, e);
            }

            return -1;
        }

        public int UpdateAutomationSchedule(AutomationSchedule schedule)
        {
            try
            {
                var existingSchedule = _automationRepository.GetScheduleEntity(schedule.Id);
                existingSchedule.ScheduleName = schedule.Name;
                existingSchedule.DayOfMonth = schedule.DayOfMonth;
                existingSchedule.DayOfWeek = schedule.DayOfWeek;
                existingSchedule.Frequency = schedule.Frequency;
                existingSchedule.ScheduleType = schedule.ScheduleType;
                existingSchedule.StartDate = schedule.StartDate.ToUniversalTime();
                existingSchedule.WeeklyTime = schedule.WeeklyTime;
                existingSchedule.IsActive = schedule.IsActive;
                existingSchedule.StopOnFail = schedule.StopOnFail;
                _automationRepository.SaveChanges();
                return 0;
            }
            catch (Exception e)
            {
                _logger.Error("Cannot add schedule to automation", e);
                OnException?.Invoke("Cannot add automation schedule", "", MessagesType.Error, e);
            }

            return -1;
        }

        public void DeleteAutomation(int id)
        {
            var entity = _automationRepository.GetAutomationEntity(id);
            if (entity != null)
            {
                _automationRepository.RemoveConfigurationHeader(entity);
                _automationRepository.SaveChanges();
                OnAutomationListUpdated?.Invoke();
            }
        }

        public void SetAutomationActiveState(int id, bool isActive)
        {
            var automation = _automationRepository.GetAutomationEntity(id);
            automation.IsActive = isActive;
            _automationRepository.SaveChanges();
        }

        public List<AutomationHeader> GetAutomationHeaders()
        {
            return _automationRepository.GetAutomation();
        }

        public AutomationConfiguration GetAutomation(int id)
        {
            return _automationRepository.GetConfiguration(id);
        }

        public List<AutomationSchedule> GetSchedules(bool onlyActive = false)
        {
            return _automationRepository.GetSchedules(onlyActive);
        }

        public List<AutomationSchedule> GetSchedules(int id)
        {
            return _automationRepository.GetSchedulesForConfiguration(id);
        }

        public void DeleteSchedule(int id)
        {
            _automationRepository.RemoveSchedule(id);
        }

        public List<AutomationLog> GetAutomationLogs()
        {
            return _automationRepository.GetLogs();
        }

        public void AddAutomationLog(AutomationLog log)
        {
            _automationRepository.AddLog(log.ToEntity());
            _automationRepository.SaveChanges();
        }
    }
}