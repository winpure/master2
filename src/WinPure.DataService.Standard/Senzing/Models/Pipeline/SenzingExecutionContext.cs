using System.Collections.Concurrent;
using System.Data;
using System.Data.SQLite;
using WinPure.DataService.Senzing.Models.G2;

namespace WinPure.DataService.Senzing.Models.Pipeline;

public class SenzingExecutionContext
{
    public SenzingExecutionContext()
    {
        PossibleDuplicates = new List<List<RelatedEntity>>();
        PossibleRelated = new List<List<RelatedEntity>>();
    }

    public Action<PipelineStepsEnum, int, int> NotifyProgress { get; set; }
    public List<EntityResolutionConfiguration> SourceData { get; set; }
    public EntityResolutionReport Report { get; set; }
    public List<long> ResolvedEntityNumbers { get; set; }
    public ConcurrentBag<string> Errors { get; set; }
    public ConcurrentBag<EntityRecord> ErrorIds { get; set; }
    public DataTable ResultTable { get; set; }
    public string ColumnList { get; set; }
    public string InsertParameters { get; set; }
    public SQLiteConnection Connection { get; set; }
    public List<List<RelatedEntity>> PossibleDuplicates { get; set; }
    public List<List<RelatedEntity>> PossibleRelated { get; set; }
    public bool LicenseLimitReached { get; set; } = false;
}