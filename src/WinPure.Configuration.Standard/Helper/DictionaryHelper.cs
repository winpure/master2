using System.Text.RegularExpressions;
using WinPure.Configuration.Properties;

namespace WinPure.Configuration.Helper;

internal static class DictionaryHelper
{
    public static string NO_DICTIONARY = Resources.COMMON_DICTIONARY_NOT_REQUIRED;


    public static bool IsWpDictionary(this string dictionaryName)
    {
        return !string.IsNullOrWhiteSpace(dictionaryName)
               && !string.Equals(dictionaryName, NO_DICTIONARY, StringComparison.InvariantCultureIgnoreCase);
    }

    public static string ApplyDictionary(this string value, Dictionary<string, string> dictionary)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }
        var result = value;

        var matchedValues = dictionary.AsParallel().Where(x => result.Contains(x.Key)).ToList();

        foreach (var dict in matchedValues)
        {
            result = Regex.Replace(result, $"\\b{dict.Key}\\b", dict.Value);
        }

        return result.Trim();
    }
}