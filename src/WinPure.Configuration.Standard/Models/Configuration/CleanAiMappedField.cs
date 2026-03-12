using WinPure.Configuration.Enums;

namespace WinPure.Configuration.Models.Configuration;

public class CleanAiMappedField
{
    public string Name { get; set; }
    public CleanAiMapType MapType { get; set; }
    public decimal Precision { get; set; }
}