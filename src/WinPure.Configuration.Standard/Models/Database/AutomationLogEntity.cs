namespace WinPure.Configuration.Models.Database
{
    public class AutomationLogEntity
    {
        public int Id { get; set; }
        public int ScheduleId { get; set; }
        public DateTime DateOfRun { get; set; }
        public bool Successful { get; set; }
        public string Message { get; set; }
        public TimeSpan ExecutionTime { get; set; }

        public AutomationScheduleEntity Schedule { get; set; }
    }
}
