namespace WinPure.Configuration.Models.Configuration;

public class CleansingAiFieldType
{
    public string AiType { get; set; }
    public List<CleanAiMappedField> MappedFields { get; set; } = new();
    public CleanAiFieldOptions Options { get; set; } = new();
}