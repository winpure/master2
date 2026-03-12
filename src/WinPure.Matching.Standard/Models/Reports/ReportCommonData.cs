using System.Reflection;

namespace WinPure.Matching.Models.Reports;

[Serializable]
[Obfuscation(Exclude = true, ApplyToMembers = true)]
public class ReportCommonData
{
    public int TotalRecords { get; set; }
    public int GroupCount { get; set; }
    public int TotalMatches { get; set; }
}