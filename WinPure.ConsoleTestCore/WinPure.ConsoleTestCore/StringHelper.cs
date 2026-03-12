using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace WinPure.ConsoleTestCore
{
    public static class StringHelper
    {

        public const string PunctuationRegex = @"[^\w\s]";
        public const string ShortWordRegex = @"\b\w{1,2}\b";
        public const string MultiSpaceRegex = @"\s+";

        private static readonly Regex RemovePunctuation = new Regex(PunctuationRegex, RegexOptions.Compiled);
        private static readonly Regex RemoveShortWords = new Regex(ShortWordRegex, RegexOptions.Compiled);
        private static readonly Regex RemoveMultiSpace = new Regex(MultiSpaceRegex, RegexOptions.Compiled);

        public static string NormalizeValueForSearch(this string stringToNormalize)
        {
            var result = RemovePunctuation.Replace(stringToNormalize, " ");
            result = RemoveShortWords.Replace(result, "");
            result = RemoveMultiSpace.Replace(result, " ");
            return result.ToLower().Trim();
        }

        private static void NormalizeData(DataTable newData, string searchField)
        {
            var normalizedColumnName = $"{searchField}_Normalized";
            var normalizedColumn = new DataColumn(normalizedColumnName, typeof(string));
            newData.Columns.Add(normalizedColumn);

            newData.AsEnumerable().AsParallel().ForAll(x => x.SetField(normalizedColumnName, x.Field<string>(searchField).NormalizeValueForSearch()));
        }
    }
}