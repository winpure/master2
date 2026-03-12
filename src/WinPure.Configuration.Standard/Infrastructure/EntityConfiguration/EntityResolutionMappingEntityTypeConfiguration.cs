using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WinPure.Configuration.Infrastructure.EntityConfiguration;

public class EntityResolutionMappingEntityTypeConfiguration : IEntityTypeConfiguration<EntityResolutionMappingEntity>
{
    public void Configure(EntityTypeBuilder<EntityResolutionMappingEntity> builder)
    {
        builder.ToTable("EntityResolutionMapping");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.DataColumnName).IsRequired();
        builder.Property(o => o.EntityType);
        builder.Property(o => o.ExactMatch).IsRequired();
        builder.Property(o => o.UsageGroup);
        builder.Property(o => o.ConflictEntityTypes);
        builder.Property(o => o.PrerequisiteEntityTypes);
    }
}