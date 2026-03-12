namespace WinPure.Matching.Models;

/// <summary>
/// Describe matching field.
/// </summary>
[Serializable]
public class MatchField
{
    /// <summary>
    /// Table, which contains matching field. Should be similar to TableName from TableParameter class. 
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// Field name in the table
    /// </summary>
    public string ColumnName { get; set; }

    /// <summary>
    /// String with system data type of the column (i.e. System.String)
    /// </summary>
    public string ColumnDataType { get; set; }
}