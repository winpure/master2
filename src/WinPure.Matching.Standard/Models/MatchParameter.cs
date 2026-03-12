using Newtonsoft.Json;

namespace WinPure.Matching.Models;

/// <summary>
/// Define matching parameters
/// </summary>
[Serializable]
public class MatchParameter : MatchParameterBase
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public MatchParameter()
    {
        Groups = new List<MatchGroup>();
    }

    public MatchParameter(MatchParameter param)
    {
        CheckInternal = param.CheckInternal;
        FuzzyAlgorithm = param.FuzzyAlgorithm;
        MainTable = param.MainTable;
        SearchDeep = param.SearchDeep;

        Groups = new List<MatchGroup>();
        // that is really stupid code but usual ToList() do not really copy all internal structure but we need full deep copy of the base class. 
        for (var g = 0; g < param.Groups.Count; g++)
        {
            var matchGroup = param.Groups[g];
            var grp = new MatchGroup
            {
                GroupId = matchGroup.GroupId,
                GroupLevel = matchGroup.GroupLevel
            };

            for (var c = 0; c < matchGroup.Conditions.Count; c++)
            {
                var matchCondition = matchGroup.Conditions[c];
                var cond = new MatchCondition
                {
                    DictionaryType = matchCondition.DictionaryType,
                    IncludeNullValues = matchCondition.IncludeNullValues,
                    IncludeEmpty = matchCondition.IncludeEmpty,
                    Level = matchCondition.Level,
                    MatchingType = matchCondition.MatchingType,
                    Weight = matchCondition.Weight
                };

                for (var f = 0; f < matchCondition.Fields.Count; f++)
                {
                    var matchFiled = matchCondition.Fields[f];
                    var fld = new MatchField
                    {
                        ColumnName = matchFiled.ColumnName,
                        TableName = matchFiled.TableName,
                        ColumnDataType = matchFiled.ColumnDataType
                    };
                    cond.Fields.Add(fld);
                }

                grp.Conditions.Add(cond);
            }

            Groups.Add(grp);
        }
    }

    /// <summary>
    /// Matching groups
    /// </summary>
    public List<MatchGroup> Groups { get; set; }

    /// <summary>
    /// Where data from multiple tables are compared: True - data inside table should be matched (require more time). False - math data only between the tables.
    /// </summary>
    public bool CheckInternal { get; set; }

    /// <summary>
    /// If CheckInternal set to FALSE, that parameter define which table will be taken as primary (or main) table for matching
    /// </summary>
    public string MainTable { get; set; }

    /// <summary>
    /// Determine how deep matching algorithm should compare data. Recommended value is 10.
    /// </summary>
    public int SearchDeep { get; set; } = 10;

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}