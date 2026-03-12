using System.Text.RegularExpressions;

namespace WinPure.Cleansing.Helpers;

internal static class StringCleanerAndConvertor
{
    private static readonly Regex RemoveLettersPattern = new Regex(@"[^0-9\s',.@/\\+=*?_!#$%^&)(\[\]{}`~-]", RegexOptions.Compiled);
    private static readonly Regex RemoveDigitsPattern = new Regex("[0-9]", RegexOptions.Compiled);
    private static readonly Regex RemoveNonPrintablePattern = new Regex(@"([^ -~])\1{0,}", RegexOptions.Compiled);
    private static readonly Regex RemoveMultiSpacePattern = new Regex(@"\s+", RegexOptions.Compiled);
    private static readonly Regex RemoveTabsPattern = new Regex(@"\t", RegexOptions.Compiled);
    private static readonly Regex RemovePunctuationPattern = new Regex(@"\p{P}", RegexOptions.Compiled);
    private static readonly Regex RemoveNewLinePattern = new Regex(Environment.NewLine, RegexOptions.Compiled);
    private static readonly Regex RemoveNewLinePattern2 = new Regex(@"\r", RegexOptions.Compiled);

    public static string ConvertNaughtsToOs(string str)
    {
        return str.Replace("0", "o");
    }

    public static string ConvertOnesToLs(string str)
    {
        return str.Replace("1", "l");
    }

    public static string ConvertOsToNaughts(string str)
    {
        return str.Replace("O", "0").Replace("o", "0");
    }

    public static string ConvertLsToOnes(string str)
    {
        return str.Replace("L", "1").Replace("l", "1");
    }

    public static string RemoveAllDigits(string str)
    {
        return RemoveDigitsPattern.Replace(str, "");
    }

    public static string RemoveAllSpaces(string str)
    {
        return str.Replace(" ", "");
    }

    public static string RemoveAllLetters(string str)
    {
        return RemoveLettersPattern.Replace(str, "");
    }

    public static string RemoveApostrophes(string str)
    {
        return str.Replace("'", "");
    }

    public static string RemoveCommas(string str)
    {
        return str.Replace(",", "").Replace("‚", ""); 
    }

    public static string RemoveDots(string str)
    {
        return str.Replace(".", "");
    }

    public static string RemoveHyphens(string str)
    {
        return str.Replace("-", "");
    }

    public static string RemoveLeadingSpace(string str)
    {
        return str.TrimStart();
    }

    public static string RemoveNonPrintableCharacters(string str, string replaceStr)
    {
        return RemoveNonPrintablePattern.Replace(str, replaceStr);
    }

    public static string RemoveTrailingSpace(string val)
    {
        return val.TrimEnd();
    }

    public static string RemoveMultipleSpace(string val)
    {
        return RemoveMultiSpacePattern.Replace(val, " ");
    }

    public static string RemoveTabs(string val)
    {
        return RemoveTabsPattern.Replace(val, "");
    }

    public static string RemoveNewLine(string val)
    {
        val = RemoveNewLinePattern.Replace(val, "");
        return RemoveNewLinePattern2.Replace(val, "");
    }

    public static string RemovePunctuation(string val)
    {
        return RemovePunctuationPattern.Replace(val, "");
    }
}