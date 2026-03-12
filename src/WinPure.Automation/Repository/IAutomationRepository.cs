using WinPure.Configuration.Models.Database;

namespace WinPure.Automation.Repository;

internal interface IAutomationRepository
{
    void SaveChanges();
    List<AutomationHeader> GetAutomation();
    AutomationHeaderEntity GetAutomationEntity(string name);
    AutomationHeaderEntity GetAutomationEntity(int id);
    AutomationHeaderEntity GetAutomationEntityWithAllDetails(int id);
    void AddConfiguration(AutomationHeaderEntity automationHeader);
    void RemoveConfigurationHeader(AutomationHeaderEntity automationHeader);
    AutomationConfiguration GetConfiguration(int id);
    List<AutomationSchedule> GetSchedules(bool onlyActive = false);
    List<AutomationSchedule> GetSchedulesForConfiguration(int configurationId);
    AutomationScheduleEntity GetScheduleEntity(int id);
    void AddSchedule(AutomationScheduleEntity automationSchedule);
    void RemoveSchedule(int id);
    void RemoveSchedule(AutomationScheduleEntity scheduleEntity);
    void RemoveConfigurationStep(AutomationConfigurationStepEntity automationConfigurationStep);
    void RemoveLog(AutomationLogEntity automationLog);
    List<AutomationLog> GetLogs();
    void AddLog(AutomationLogEntity automationLogEntity);
}