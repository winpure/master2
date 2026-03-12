using WinPure.Automation.Enums;
using WinPure.Configuration.Models.Database;

namespace WinPure.Automation.Models;

public class AutomationStep
{
    public int Id { get; set; }
    public int ConfigurationId { get; set; }
    public int Order { get; set; }
    public AutomationStepType StepType { get; set; }
    public string SourceName { get; set; }
    public string Param1 { get; set; }
    public string Param2 { get; set; }

    public AutomationConfigurationStepEntity ToEntity()
    {
        return new AutomationConfigurationStepEntity
        {
            Parameter1 = Param1,
            Parameter2 = Param2,
            SourceName = SourceName,
            ConfigurationId = ConfigurationId,
            StepId = (int)StepType
        };
    }
}