namespace WinPure.Cleansing.Models.Statistic;

internal class ColumnStatistic
{
    public string ColumnName { get; set; }
    public int Order { get; set; }
    public string ColumnType { get; set; }

    public string MostCommon { get; set; }
    public double MaxNumber { get; set; } = double.MinValue;
    public double MinNumber { get; set; } = double.MaxValue;
    public int MaxWords { get; set; }
    public int MaxLength { get; set; }
    public int CountNull { get; set; }
    public int CountNotNull { get; set; }
    public long Distinct { get; set; }
    public long TotalLength { get; set; }
    public long MostCommonCount { get; set; }
    public long Numbers { get; set; }
    public long NewLineChar { get; set; }
    public long ProperCase { get; set; }
    public long TotalWords { get; set; }
    public long NotPrintable { get; set; }
    public long Hyphens { get; set; }
    public long MixedCase { get; set; }
    public long Punctuation { get; set; }
    public long TabChar { get; set; }
    public long Dots { get; set; }
    public long Commas { get; set; }
    public long LeadingSpaces { get; set; }
    public long Apostrophes { get; set; }
    public long TrailingSpaces { get; set; }
    public long MultipleSpaces { get; set; }
    public long WithSpaces { get; set; }
    public long Letters { get; set; }
    public long UpperOnly { get; set; }
    public long LowerOnly = 0;
    public double AvgWords { get; set; }
    public double AvgLength { get; set; }
    public int MatchPattern { get; set; }
    public int UnmatchPattern { get; set; }
}