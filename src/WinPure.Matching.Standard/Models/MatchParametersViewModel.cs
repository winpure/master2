using WinPure.Configuration.Helper;

namespace WinPure.Matching.Models;

[Serializable]
public class MatchParametersViewModel
{
    public MatchParametersViewModel(FieldMapping map)
    {
        FieldMap = map;
        Level = 95;
        GroupId = 1;
        GroupLevel = 95;
        Dictionary = DictionaryHelper.NO_DICTIONARY;
        IsDirect = false;
        IsFuzzy = true;
        IncludeNullValue = false;
        IncludeEmpty = false;
        Weight = 100;
    }
    public string FieldName => FieldMap.FieldName;
    public FieldMapping FieldMap { get; set; }
    public bool IsFuzzy { get; set; }
    public bool IsDirect { get; set; }
    public bool IncludeNullValue { get; set; }
    public bool IncludeEmpty { get; set; }
    public double Level { get; set; }
    public string Dictionary { get; set; }
    public int GroupLevel { get; set; }
    public int Weight { get; set; }
    public int GroupId { get; set; }
}