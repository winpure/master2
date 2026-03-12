namespace WinPure.Configuration.Models.Database
{
    public class AutomationStepDictionaryEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<AutomationConfigurationStepEntity> ConfigurationSteps { get; set; }
    }
}
