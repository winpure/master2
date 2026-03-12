namespace WinPure.Cleansing.Models;

/// <summary>
/// Split column depends from meaning. Only one property should be filled. 
/// </summary>
[Serializable]
public class ColumnSplitSetting : BaseCleansingSettings
{
    /// <summary>
    /// split string into the separate words
    /// </summary>
    public SplitIntoWords SplitIntoWords;

    /// <summary>
    /// Split telephone number to international code and number
    /// </summary>
    public bool SplitTelephoneIntoInternationalCodeAndPhoneNumber;

    /// <summary>
    /// Split datetime to the parts
    /// </summary>
    public bool SplitDatetime;

    /// <summary>
    /// split name and email into the two parts
    /// </summary>
    public bool SplitNameAndEmailAddress;

    /// <summary>
    /// Split email on the parts
    /// </summary>
    public bool SplitEmailAddressIntoAccountDomainAndZone;

    /// <summary>
    /// Regular expression (REGEX) for extract substring from data by template
    /// </summary>
    public string RegexCopy { get; set; }
}


/// <summary>
/// split string into the separate words
/// </summary>
public class SplitIntoWords
{
    /// <summary>
    /// split sting into words by specific char or string
    /// </summary>
    public string SplitSeparator { get; set; }
}