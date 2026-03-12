using WinPure.Configuration.Models.Database;

namespace WinPure.Automation.Models;

public class AutomationLog
{
    public int Id { get; set; }
    public int ScheduleId { get; set; }
    public string ScheduleName { get; set; }
    public int ConfigurationId { get; set; }
    public string ConfigurationName { get; set; }
    public string Message { get; set; }
    public DateTime StartDate { get; set; }

    public bool Successed { get; set; }
    public TimeSpan ExecutionTime { get; set; }

    public AutomationLogEntity ToEntity()
    {
        return new AutomationLogEntity
        {
            Message = Message,
            ExecutionTime = ExecutionTime,
            ScheduleId = ScheduleId,
            DateOfRun = StartDate,
            Successful = Successed
        };
    }
}