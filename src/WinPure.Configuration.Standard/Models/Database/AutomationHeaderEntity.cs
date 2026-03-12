namespace WinPure.Configuration.Models.Database
{
    public class AutomationHeaderEntity
    {
        public int Id { get; set; }
        public string ConfigurationName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationDate { get; set; }

        public List<AutomationConfigurationStepEntity> ConfigurationSteps { get; set; }
        public List<AutomationScheduleEntity> Schedules { get; set; }
    }
}
