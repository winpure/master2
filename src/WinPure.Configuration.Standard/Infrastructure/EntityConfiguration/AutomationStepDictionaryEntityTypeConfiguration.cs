using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WinPure.Configuration.Infrastructure.EntityConfiguration
{
    public class AutomationStepDictionaryEntityTypeConfiguration : IEntityTypeConfiguration<AutomationStepDictionaryEntity>
    {
        public void Configure(EntityTypeBuilder<AutomationStepDictionaryEntity> builder)
        {
            builder.ToTable("AutomationStepsDictionary");

            builder.HasKey(o => o.Id);

            builder.Property(e => e.Name).IsRequired();

            builder.HasMany(o => o.ConfigurationSteps).WithOne(s => s.Step);
        }
    }
}