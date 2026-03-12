using WinPure.Cleansing.Enums;

namespace WinPure.Cleansing.Models;

/// <summary>
/// Word manager parameters
/// </summary>
[Serializable]
public class WordManagerSetting : BaseCleansingSettings
{
    /// <summary>
    /// value to search in the column
    /// </summary>
    public string SearchValue { set; get; }

    /// <summary>
    /// should be value deleted?
    /// </summary>
    public bool ToDelete { set; get; }

    /// <summary>
    /// string for replacing original value
    /// </summary>
    public string ReplaceValue { set; get; }

    /// <summary>
    /// how the original value will be searched (whole word / any part / whole value)
    /// </summary>
    public WordManagerReplaceType ReplaceType { set; get; }
}