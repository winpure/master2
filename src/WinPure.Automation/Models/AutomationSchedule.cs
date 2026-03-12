using WinPure.Configuration.Enums;
using WinPure.Configuration.Models.Database;

namespace WinPure.Automation.Models;

public class AutomationSchedule
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ConfigurationId { get; set; }
    public string ConfigurationName { get; set; }
    public DateTime StartDate { get; set; }
    public AutomationScheduleType ScheduleType { get; set; }
    public int? Frequency { get; set; }
    public short? DayOfMonth { get; set; }
    public string DayOfWeek { get; set; }
    public DateTime WeeklyTime { get; set; }
    public bool IsActive { get; set; }
    public bool StopOnFail { get; set; }

    public AutomationScheduleEntity ToEntity()
    {
        return new AutomationScheduleEntity
        {
            IsActive = IsActive,
            WeeklyTime = WeeklyTime,
            Frequency = Frequency,
            ScheduleName = Name,
            ConfigurationId = ConfigurationId,
            StartDate = StartDate.ToUniversalTime(),
            ScheduleType = ScheduleType,
            DayOfMonth = DayOfMonth,
            DayOfWeek = DayOfWeek,
            StopOnFail = StopOnFail
        };
    }
}