using System.Reflection;

namespace WinPure.Matching.Models.Reports;

[Serializable]
[Obfuscation(Exclude = true, ApplyToMembers = true)]
public class ReportSourceData
{
    public string Description { get; set; }
    public int RecordsCount { get; set; }
    public int Duplicates { get; set; }
    public int MatchedRecords { get; set; }
}