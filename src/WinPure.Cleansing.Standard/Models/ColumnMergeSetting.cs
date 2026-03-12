namespace WinPure.Cleansing.Models;

/// <summary>
/// Column merger settings
/// </summary>
[Serializable]
public class ColumnMergeSetting : BaseCleansingSettings
{
    public ColumnMergeSetting()
    {
        CharToInsertBetweenColumn = " ";
    }

    /// <summary>
    /// character or string which will divide existing column values in the result string.
    /// </summary>
    public string CharToInsertBetweenColumn { get; set; }
    /// <summary>
    /// Define order in which values would be merged in result
    /// </summary>
    public int Order { get; set; }
}