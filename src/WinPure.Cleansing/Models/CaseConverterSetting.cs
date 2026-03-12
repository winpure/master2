namespace WinPure.Cleansing.Models;

/// <summary>
/// case converter settings. Only one option should be enabled.
/// </summary>
[Serializable]
public class CaseConverterSetting : BaseCleansingSettings
{
    /// <summary>
    /// convert string to UPPER case
    /// Example: John sMith => JOHN SMITH
    /// </summary>
    public bool ToUpperCase { get; set; }

    /// <summary>
    /// convert string to lover case
    /// </summary>
    public bool ToLowerCase { get; set; }

    /// <summary>
    /// Convert String To Proper Case
    /// </summary>
    public bool ToProperCase { get; set; }

    /// <summary>
    /// Define rules for proper case conversion
    /// </summary>
    public ProperCaseSettings ProperCaseSettings { get; set; }
}