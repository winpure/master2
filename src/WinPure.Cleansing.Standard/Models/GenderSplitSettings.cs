namespace WinPure.Cleansing.Models;

public class GenderSplitSettings : BaseCleansingSettings
{
    public GenderSplitSettings()
    {
        GenderColumns = new List<string>();
    }
    /// <summary>
    /// List of the columns which contains all gender information
    /// </summary>
    public List<string> GenderColumns { get; set; }
}