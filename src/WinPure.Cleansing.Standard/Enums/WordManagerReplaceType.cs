namespace WinPure.Cleansing.Enums;

/// <summary>
/// Word manager replace type
/// </summary>
public enum WordManagerReplaceType
{
    /// <summary>
    /// replace if given SearchValue is separate word
    /// </summary>
    WholeWord = 0,
    /// <summary>
    /// replace if given SearchValue is any part of the word or string (including separate word)
    /// </summary>
    AnyPart = 1,
    /// <summary>
    /// replace if given SearchValue is a value itself
    /// </summary>
    WholeValue = 2
}