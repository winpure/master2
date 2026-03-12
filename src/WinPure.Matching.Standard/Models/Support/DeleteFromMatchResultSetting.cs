namespace WinPure.Matching.Models.Support;

/// <summary>
/// Delete from match result settings
/// </summary>
[Serializable]
public class DeleteFromMatchResultSetting
{
    /// <summary>
    /// Delete settings
    /// </summary>
    public DeleteMatchResultSetting DeleteSetting { get; set; }
}