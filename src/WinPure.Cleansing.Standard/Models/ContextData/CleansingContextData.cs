namespace WinPure.Cleansing.Models.ContextData;

internal class CleansingContextData
{
    public long WinPureId { get; set; }
    public string Value { get; set; }
    public string GenderSplitValue { get; set; }
    public List<string> MergeValues { get; set; }
    public string MergeResult { get; set; }
    public TextCheckerResult TextCheckerResult { get; set; }
    public SplitResult SplitResult { get; set; }
    public SplitAddressResult SplitAddressResult { get; set; }
    public SplitGenderResult SplitGenderResult { get; set; }
}