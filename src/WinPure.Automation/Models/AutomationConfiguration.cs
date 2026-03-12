using System.Linq;
using WinPure.Configuration.Models.Database;

namespace WinPure.Automation.Models;

public class AutomationConfiguration : AutomationHeader
{
    public AutomationConfiguration()
    {
        Steps = new List<AutomationStep>();
        CreationDate = DateTime.Now;
        IsActive = true;
    }
    public List<AutomationStep> Steps { get; set; }

    public AutomationHeaderEntity ToEntity()
    {
        var entity = new AutomationHeaderEntity
        {
            //Id = Id,
            ConfigurationName = Name,
            CreationDate = CreationDate.ToUniversalTime(),
            IsActive = IsActive,
            ConfigurationSteps = Steps.OrderBy(x => x.StepType).ThenBy(x => x.Order).Select(x => x.ToEntity()).ToList()
        };

        return entity;
    }
}