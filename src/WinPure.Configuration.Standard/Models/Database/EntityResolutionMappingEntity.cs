namespace WinPure.Configuration.Models.Database;

public class EntityResolutionMappingEntity
{
    public int Id { get; set; }
    public string DataColumnName { get; set; }
    public string EntityType { get; set; }
    public bool ExactMatch { get; set; }
    public string UsageGroup { get; set; }
    public string ConflictEntityTypes { get; set; }
    public string PrerequisiteEntityTypes { get; set; }

    public EntityResolutionMappingSetting ToSettings() =>
         new EntityResolutionMappingSetting
         {
             Id = Id,
             EntityType = EntityType,
             DataColumnName = DataColumnName,
             ExactMatch = ExactMatch,
             UsageGroup = UsageGroup,
             ConflictEntityTypes = string.IsNullOrWhiteSpace(ConflictEntityTypes)
                ? new List<string>()
                : ConflictEntityTypes.Split('|').Where(v => !string.IsNullOrEmpty(v)).ToList(),
             PrerequisiteEntityTypes = string.IsNullOrWhiteSpace(PrerequisiteEntityTypes)
                ? new List<string>()
                : PrerequisiteEntityTypes.Split('|').Where(v => !string.IsNullOrEmpty(v)).ToList()
         };
}