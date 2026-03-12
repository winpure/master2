using System.Reflection;

namespace WinPure.Common.Models
{
    //TODO Rename
    [Serializable]
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public class ReportResultData
    {
        public string Description { get; set; }
        public int RecordValue { get; set; }
    }
}