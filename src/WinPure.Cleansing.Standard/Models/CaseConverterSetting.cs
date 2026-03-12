namespace WinPure.Cleansing.Models;

/// <summary>
/// case converter settings. Only one option should be enabled.
/// </summary>
[Serializable]
public class CaseConverterSetting : BaseCleansingSettings
{
    /// <summary>
    /// convert string to UPPER case
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
}