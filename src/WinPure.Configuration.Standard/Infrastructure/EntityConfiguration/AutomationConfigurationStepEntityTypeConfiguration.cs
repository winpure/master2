using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WinPure.Configuration.Infrastructure.EntityConfiguration
{
    public class AutomationConfigurationStepEntityTypeConfiguration : IEntityTypeConfiguration<AutomationConfigurationStepEntity>
    {
        public void Configure(EntityTypeBuilder<AutomationConfigurationStepEntity> builder)
        {
            builder.ToTable("AutomationConfigurationSteps");

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Parameter1);
            builder.Property(o => o.Parameter2);
            builder.Property(o => o.SourceName);
            builder.Property(o => o.StepId).IsRequired();
            builder.Property(o => o.ConfigurationId).IsRequired();

            builder.HasOne(o => o.Step).WithMany(s => s.ConfigurationSteps);
            builder.HasOne(o => o.Configuration).WithMany(s => s.ConfigurationSteps);
        }

    }
}