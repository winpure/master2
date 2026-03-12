using System.Text.RegularExpressions;

namespace WinPure.Common.Helpers;

public static class RegexHelper
{
    public static bool IsValidRegex(string pattern)
    {
        try
        {
            var regex = new Regex(pattern);
            return true;
        }
        catch
        {
            return false;
        }
    }
}