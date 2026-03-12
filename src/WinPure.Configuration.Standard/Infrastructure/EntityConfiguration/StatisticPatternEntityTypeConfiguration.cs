using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WinPure.Configuration.Infrastructure.EntityConfiguration;

public class StatisticPatternEntityTypeConfiguration : IEntityTypeConfiguration<StatisticPatternEntity>
{
    public void Configure(EntityTypeBuilder<StatisticPatternEntity> builder)
    {
        builder.ToTable("StatisticPatterns");

        builder.HasKey(o => o.Id);
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Pattern).IsRequired();
        builder.HasIndex(x => x.Pattern).IsUnique();
    }
}