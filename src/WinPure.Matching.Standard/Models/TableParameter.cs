using System.Data;

namespace WinPure.Matching.Models;

/// <summary>
/// Describe data table
/// </summary>
public class TableParameter
{
    /// <summary>
    /// Table name
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// Table data
    /// </summary>
    public DataTable TableData { get; set; }
}