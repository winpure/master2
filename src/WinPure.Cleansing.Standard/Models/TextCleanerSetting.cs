namespace WinPure.Cleansing.Models;

/// <summary>
/// clean string from extra characters 
/// </summary>
[Serializable]
public class TextCleanerSetting : BaseCleansingSettings
{
    /// <summary>
    /// Remove non=printable characters
    /// </summary>
    public bool RemoveNonPrintableCharacters { get; set; }

    /// <summary>
    /// remove all digits from string
    /// </summary>
    public bool RemoveAllDigits { get; set; }

    /// <summary>
    /// remove any given character or substring from the string
    /// </summary>
    public string RemoveCharacters { get; set; }

    /// <summary>
    /// remove all characters, except of punctuation.
    /// </summary>
    public bool RemoveAllLetters { get; set; }

    /// <summary>
    /// remove all spaces from string
    /// </summary>
    public bool RemoveAllSpaces { get; set; }

    /// <summary>
    /// remove dots
    /// </summary>
    public bool RemoveDots { get; set; }

    /// <summary>
    /// remove commas
    /// </summary>
    public bool RemoveCommas { get; set; }

    /// <summary>
    /// remove Hyphens
    /// </summary>
    public bool RemoveHyphens { get; set; }

    /// <summary>
    /// remove  Apostrophes
    /// </summary>
    public bool RemoveApostrophes { get; set; }

    /// <summary>
    /// remove leading spaces
    /// </summary>
    public bool RemoveLeadingSpace { get; set; }

    /// <summary>
    /// remove trailing spaces
    /// </summary>
    public bool RemoveTrailingSpace { get; set; }

    /// <summary>
    /// Convert O to 0 
    /// </summary>
    public bool ConvertOsToNaughts { get; set; }

    /// <summary>
    /// Convert L to 1
    /// </summary>
    public bool ConvertLsToOnes { get; set; }

    /// <summary>
    /// Convert 0 to O
    /// </summary>
    public bool ConvertNaughtsToOs { get; set; }

    /// <summary>
    /// convert 1 to L
    /// </summary>
    public bool ConvertOnesToLs { get; set; }

    /// <summary>
    /// Replace NULL and empty values with some default value. 
    /// </summary>
    public string ConvertEmptyToDefaultValue { get; set; }

    /// <summary>
    /// Normalize string with removing all extra spaces between words
    /// </summary>
    public bool RemoveMultipleSpaces { get; set; }

    /// <summary>
    /// Remove Tabulation symbol from the string
    /// </summary>
    public bool RemoveTabs { get; set; }

    /// <summary>
    /// Remove New line symbol from the string
    /// </summary>
    public bool RemoveNewLine { get; set; }

    /// <summary>
    /// Remove any kind of punctuation character according to Regex definition of punctuation characters (https://en.wikipedia.org/wiki/Regular_expression)
    /// </summary>
    public bool RemovePunctuation { get; set; }

    /// <summary>
    /// define a regular expression (REGEX) pattern to find in source data (found value would be replaced with value from RegexReplace field)
    /// </summary>
    public string RegexExpression { get; set; }

    /// <summary>
    /// define value for replacing using REGEX
    /// </summary>
    public string RegexReplace { get; set; }
}