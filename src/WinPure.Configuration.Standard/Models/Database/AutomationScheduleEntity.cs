using WinPure.Configuration.Enums;

namespace WinPure.Configuration.Models.Database
{
    public class AutomationScheduleEntity
    {
        public int Id { get; set; }
        public string ScheduleName { get; set; }
        public int ConfigurationId { get; set; }
        public AutomationScheduleType ScheduleType { get; set; }
        public DateTime StartDate { get; set; }
        public int? Frequency { get; set; }
        public string DayOfWeek { get; set; }
        public short? DayOfMonth { get; set; }
        public DateTime WeeklyTime { get; set; }
        public bool IsActive { get; set; }
        public bool StopOnFail { get; set; }

        public AutomationHeaderEntity Configuration { get; set; }
        public List<AutomationLogEntity> Logs { get; set; }
    }
}
