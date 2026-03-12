namespace WinPure.Cleansing.Models;

/// <summary>
/// Data check settings
/// </summary>
[Serializable]
public class ColumnCheckSettings : BaseCleansingSettings
{
    /// <summary>
    /// Verify if data is valid email
    /// </summary>
    public bool CheckEmail { get; set; }
}