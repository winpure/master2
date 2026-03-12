namespace WinPure.Cleansing.Models;

/// <summary>
/// Shift setting
/// </summary>
[Serializable]
public class ColumnShiftSetting : BaseCleansingSettings
{
    /// <summary>
    /// column index in source table. 
    /// </summary>
    public int SourceIndex { get; set; }

    /// <summary>
    /// if true then shift left otherwise shift right.
    /// </summary>
    public bool ShiftLeft { get; set; }
}