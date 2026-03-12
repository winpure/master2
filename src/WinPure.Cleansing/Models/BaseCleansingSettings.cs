namespace WinPure.Cleansing.Models;

[Serializable]
public abstract class BaseCleansingSettings
{
    /// <summary>
    /// Column Name for the clean operation
    /// </summary>
    public string ColumnName { get; set; }
}