using System.Data;

namespace WinPure.Matching.Models.Support;

internal class MasterRecordItem
{
    public int GroupId { get; set; }
    public int FilledColumnsCount { get; set; }
    public byte IsPreferredTable { get; set; }
    public long PrimK { get; set; }
    public bool IsMaster { get; set; }
    public DataRow DataRow { get; set; }
}