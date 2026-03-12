namespace WinPure.Configuration.Models.Configuration;

public class StatisticPatternSetting
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Pattern { get; set; }
    public string Description { get; set; }
    public string FieldType { get; set; }

    public StatisticPatternEntity ToEntity() => new StatisticPatternEntity
    {
        Id = Id,
        FieldType = FieldType,
        Name = Name,
        Pattern = Pattern,
        Description = Description
    };
}