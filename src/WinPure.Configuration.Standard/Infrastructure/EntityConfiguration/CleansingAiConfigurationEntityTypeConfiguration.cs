using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace WinPure.Configuration.Infrastructure.EntityConfiguration;

internal class CleansingAiConfigurationEntityTypeConfiguration : IEntityTypeConfiguration<CleansingAiConfigurationEntity>
{
    public void Configure(EntityTypeBuilder<CleansingAiConfigurationEntity> builder)
    {
        builder.ToTable("CleansingAiConfigurations");

        builder.HasKey(e => e.AiType);

        builder.Property(e => e.Options).IsRequired()
            .HasColumnType("TEXT")
            .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<CleanAiFieldOptions>(v));

        builder.Property(e => e.MappedFields).IsRequired()
            .HasColumnType("TEXT")
            .HasConversion(v => JsonConvert.SerializeObject(v), v => JsonConvert.DeserializeObject<List<CleanAiMappedField>>(v));
    }
}