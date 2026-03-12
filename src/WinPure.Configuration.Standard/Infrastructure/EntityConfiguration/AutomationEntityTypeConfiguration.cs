using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WinPure.Configuration.Infrastructure.EntityConfiguration
{
    public class AutomationEntityTypeConfiguration : IEntityTypeConfiguration<AutomationConfigurationStepEntity>
    {
        public void Configure(EntityTypeBuilder<AutomationConfigurationStepEntity> builder)
        {
            builder.ToTable("AutomationConfigurationSteps");

            builder.HasKey(o => o.Id);
            builder.Property(o => o.SourceName).IsRequired();
            builder.Property(o => o.StepId).IsRequired();

            builder.HasOne(x => x.Configuration)
                .WithMany(x => x.ConfigurationSteps)
                .HasForeignKey(x => x.ConfigurationId);

            builder.HasOne(x => x.Step)
                .WithMany(x => x.ConfigurationSteps)
                .HasForeignKey(x => x.StepId);
        }

    }
}