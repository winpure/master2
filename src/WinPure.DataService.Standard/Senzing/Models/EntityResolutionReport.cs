namespace WinPure.DataService.Senzing.Models;

public class EntityResolutionReport
{
    public EntityResolutionReport()
    {
        GroupWithDuplicates = new List<long>();
        GroupUnique = new List<long>();
    }

    public int TotalRecords { get; set; }
    public int DuplicateRecordsCount { get; set; }
    public int PossibleDuplicateRecordsCount { get; set; }
    public int PossibleDuplicateGroupCount { get; set; }
    public int RelatedGroupCount { get; set; }
    public int RelatedRecordsCount { get; set; }
    public List<long> GroupWithDuplicates { get; set; }
    public List<long> GroupUnique { get; set; }
    public TimeSpan ExecutionTime { get; set; }
}