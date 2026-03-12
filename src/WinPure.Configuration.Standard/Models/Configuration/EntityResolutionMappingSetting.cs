namespace WinPure.Configuration.Models.Configuration;

public class EntityResolutionMappingSetting
{
    public int Id { get; set; }
    public string DataColumnName { get; set; }
    public string EntityType { get; set; }
    public bool ExactMatch { get; set; }
    public string UsageGroup { get; set; }
    public List<string> ConflictEntityTypes { get; set; }
    public List<string> PrerequisiteEntityTypes { get; set; }

    public EntityResolutionMappingEntity ToEntity()
    {
        return new EntityResolutionMappingEntity
        {
            Id = Id,
            EntityType = EntityType,
            ExactMatch = ExactMatch,
            DataColumnName = DataColumnName,
            UsageGroup = UsageGroup,
            ConflictEntityTypes = String.Join("|", ConflictEntityTypes),
            PrerequisiteEntityTypes = String.Join("|", PrerequisiteEntityTypes),
        };
    }
}