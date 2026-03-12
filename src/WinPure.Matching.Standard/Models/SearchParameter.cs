using Newtonsoft.Json;

namespace WinPure.Matching.Models;

/// <summary>
/// Define search parameter
/// </summary>
[Serializable]
public class SearchParameter : MatchParameterBase
{

    public SearchParameter()
    {
        Groups = new List<SearchGroup>();
    }

    public SearchParameter(SearchParameter param)
    {
        FuzzyAlgorithm = param.FuzzyAlgorithm;

        Groups = new List<SearchGroup>();
        // that is really stupped code but usual ToList() do not really copy all internal structure but we need full deep copy of the base class. 
        for (var g = 0; g < param.Groups.Count; g++)
        {
            var matchGroup = param.Groups[g];
            var grp = new SearchGroup
            {
                GroupId = matchGroup.GroupId,
                GroupLevel = matchGroup.GroupLevel
            };

            for (var c = 0; c < matchGroup.Conditions.Count; c++)
            {
                var matchCondition = matchGroup.Conditions[c];
                var cond = new SearchCondition
                {
                    DictionaryType = matchCondition.DictionaryType,
                    IncludeNullValues = matchCondition.IncludeNullValues,
                    IncludeEmpty = matchCondition.IncludeEmpty,
                    Level = matchCondition.Level,
                    MatchingType = matchCondition.MatchingType,
                    Weight = matchCondition.Weight,
                    SearchValue = matchCondition.SearchValue,
                    SearchField = new MatchField
                    {
                        ColumnDataType = matchCondition.SearchField.ColumnDataType,
                        ColumnName = matchCondition.SearchField.ColumnName,
                        TableName = matchCondition.SearchField.TableName
                    }
                };

                grp.Conditions.Add(cond);
            }

            Groups.Add(grp);
        }
    }

    /// <summary>
    /// Search groups
    /// </summary>
    public List<SearchGroup> Groups { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}