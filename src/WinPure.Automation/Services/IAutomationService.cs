using WinPure.Common.Abstractions;

namespace WinPure.Automation.Services;

internal interface IAutomationService : IWpServiceBase
{
    event Action OnAutomationListUpdated;

    bool IsNameExists(string name);
    int AddAutomation(AutomationConfiguration configuration);
    void UpdateAutomation(AutomationConfiguration configuration);
    int AddAutomationSchedule(AutomationSchedule schedule);
    int UpdateAutomationSchedule(AutomationSchedule schedule);
    void DeleteAutomation(int id);
    void SetAutomationActiveState(int id, bool isActive);
    List<AutomationHeader> GetAutomationHeaders();
    AutomationConfiguration GetAutomation(int id);
    List<AutomationSchedule> GetSchedules(bool onlyActive = false);
    List<AutomationSchedule> GetSchedules(int id);
    void DeleteSchedule(int id);
    List<AutomationLog> GetAutomationLogs();
    void AddAutomationLog(AutomationLog log);
}