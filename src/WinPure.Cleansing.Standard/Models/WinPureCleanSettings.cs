namespace WinPure.Cleansing.Models;

/// <summary>
/// Model, that describe all required clean operations
/// </summary>
[Serializable]
public class WinPureCleanSettings
{
    public WinPureCleanSettings()
    {
        TextCleanerSettings = new List<TextCleanerSetting>();
        CaseConverterSettings = new List<CaseConverterSetting>();
        WordManagerSettings = new List<WordManagerSetting>();
        ColumnSplitSettings = new List<ColumnSplitSetting>();
        ColumnMergeSettings = new List<ColumnMergeSetting>();
        ColumnShiftSettings = new List<ColumnShiftSetting>();
        ColumnCheckSettings = new List<ColumnCheckSettings>();
        GenderSplitSettings = new GenderSplitSettings();
    }

    /// <summary>
    /// List contains all required text cleaner operations for the fields. 
    /// </summary>
    public List<TextCleanerSetting> TextCleanerSettings { set; get; }

    /// <summary>
    /// list of required case converter operations
    /// </summary>
    public List<CaseConverterSetting> CaseConverterSettings { set; get; }

    /// <summary>
    /// list of required Word manager operation (replace word or part of the word with some another value)
    /// </summary>
    public List<WordManagerSetting> WordManagerSettings { set; get; }

    /// <summary>
    /// list of column splitter operation
    /// </summary>
    public List<ColumnSplitSetting> ColumnSplitSettings { set; get; }

    /// <summary>
    /// list of columns to merge
    /// </summary>
    public List<ColumnMergeSetting> ColumnMergeSettings { set; get; }

    /// <summary>
    /// list of the columns to shift left/right
    /// </summary>
    public List<ColumnShiftSetting> ColumnShiftSettings { set; get; }

    /// <summary>
    /// Check column data 
    /// </summary>
    public List<ColumnCheckSettings> ColumnCheckSettings { set; get; }


    /// <summary>
    /// Split gender 
    /// </summary>
    public GenderSplitSettings GenderSplitSettings { get; set; }
}