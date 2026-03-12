namespace WinPure.Configuration.Models.Database
{
    public class AutomationConfigurationStepEntity
    {
        public int Id { get; set; }
        public int ConfigurationId { get; set; }
        public int StepId { get; set; }
        public string SourceName { get; set; }
        public string Parameter1 { get; set; }
        public string Parameter2 { get; set; }

        public AutomationHeaderEntity Configuration { get; set; }
        public AutomationStepDictionaryEntity Step { get; set; }
    }
}
