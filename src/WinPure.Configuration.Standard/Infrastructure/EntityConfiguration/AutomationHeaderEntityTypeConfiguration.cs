using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WinPure.Configuration.Infrastructure.EntityConfiguration
{
    public class AutomationHeaderEntityTypeConfiguration : IEntityTypeConfiguration<AutomationHeaderEntity>
    {
        public void Configure(EntityTypeBuilder<AutomationHeaderEntity> builder)
        {
            builder.ToTable("AutomationHeaders");

            builder.HasKey(o => o.Id);
            builder.Property(e => e.ConfigurationName).IsRequired();
            builder.Property(e => e.CreationDate).IsRequired();
            builder.Property(e => e.IsActive).IsRequired();

            builder.HasMany(x => x.ConfigurationSteps).WithOne(x => x.Configuration);
            builder.HasMany(x => x.Schedules).WithOne(x => x.Configuration);
        }

    }
}