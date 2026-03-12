namespace WinPure.Configuration.Models.Database;

public class StatisticPatternEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Pattern { get; set; }
    public string Description { get; set; }
    public string FieldType { get; set; }

    public StatisticPatternSetting ToSetting() => new StatisticPatternSetting
    {
        Id = Id,
        FieldType = FieldType,
        Name = Name,
        Pattern = Pattern,
        Description = Description
    };
}