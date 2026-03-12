using Newtonsoft.Json;
using System.Reflection;

namespace WinPure.Matching.Models;

[Serializable]
[Obfuscation(Exclude = true, ApplyToMembers = true)]
public class MatchSettingsViewModel
{
    public MatchSettingsViewModel()
    {
        MatchAcrossTables = false;
        AcrossTableMainTable = "";
        MatchParameters = new List<MatchParametersViewModel>();
    }

    public void InitiateSettings(MatchSettingsViewModel sett)
    {
        MatchAcrossTables = sett.MatchAcrossTables;
        AcrossTableMainTable = sett.AcrossTableMainTable;
        MatchParameters = sett.MatchParameters;
    }

    public bool MatchAcrossTables { get; set; }
    public string AcrossTableMainTable { get; set; }
    public List<MatchParametersViewModel> MatchParameters { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}