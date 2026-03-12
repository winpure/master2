namespace WinPure.Matching.Models.Support;

/// <summary>
/// Update match result settings
/// </summary>
public class UpdateMatchResultSetting : MatchResultSettingBase
{
    /// <summary>
    /// Define action how filed would be updated
    /// </summary>
    public UpdateOperationType Operation { get; set; }
    /// <summary>
    /// If True then only empty fields would be updated
    /// </summary>
    public bool OnlyEmpty { get; set; }
}