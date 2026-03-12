using Newtonsoft.Json;

namespace WinPure.Matching.Models.Support;

/// <summary>
/// Define setting to master record definition
/// </summary>
[Serializable]
public class MasterRecordSettings
{
    public MasterRecordSettings()
    {
        Rules = new List<MasterRecordRule>();
    }

    /// <summary>
    /// Define the rule how master record would be defined
    /// </summary>
    public MasterRecordType RecordType { get; set; }
    /// <summary>
    /// If two records have similar equality rtae - then select record from preferred table as master record. 
    /// </summary>
    public string PreferredTable { get; set; }
    /// <summary>
    /// Should we define master record with RecordType option if master record can not be defined with rules. 
    /// </summary>
    public bool ApplyOptionsIfRuleGiveNothing { get; set; }
    /// <summary>
    /// If all rules should be applied or any of rules (if True then all rules joined with logic AND, if False - all rules joined with logic OR)
    /// </summary>
    public bool IsAllRules { get; set; }
    /// <summary>
    /// True if we have to define master records only from the records in the Preferred table.
    /// </summary>
    public bool OnlyThisTable { get; set; }
    /// <summary>
    /// define list of rules for definition of master records
    /// </summary>
    public List<MasterRecordRule> Rules { get; set; }
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}