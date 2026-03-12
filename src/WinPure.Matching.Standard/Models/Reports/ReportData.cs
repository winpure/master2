using System.Reflection;
using WinPure.Common.Models;

namespace WinPure.Matching.Models.Reports;

[Serializable]
[Obfuscation(Exclude = true, ApplyToMembers = true)]
public class ReportData
{
    public ReportData()
    {
        ViewData = new Dictionary<MatchResultViewType, ReportCommonData>();
        CommonData = new List<ReportCommonData>();
        SourceData = new List<ReportSourceData>();
        ResultData = new List<ReportResultData>();
        MatchSettings = new MatchSettingsViewModel();
    }

    public List<ReportCommonData> CommonData { get; set; }
    public List<ReportSourceData> SourceData { get; set; }
    public List<ReportResultData> ResultData { get; set; }
    public MatchSettingsViewModel MatchSettings { get; set; }

    public Dictionary<MatchResultViewType, ReportCommonData> ViewData { get; set; }
    public TimeSpan MatchingTime { get; set; }
}