using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WinPure.Configuration.Infrastructure.EntityConfiguration
{
    public class AutomationScheduleEntityTypeConfiguration : IEntityTypeConfiguration<AutomationScheduleEntity>
    {
        public void Configure(EntityTypeBuilder<AutomationScheduleEntity> builder)
        {
            builder.ToTable("AutomationSchedules");

            builder.HasKey(o => o.Id);

            builder.Property(e => e.ScheduleName).IsRequired();

            builder.Property(e => e.ScheduleType).IsRequired();
            builder.Property(e => e.StopOnFail).HasDefaultValue(true);

            builder.HasOne(x => x.Configuration)
                .WithMany(x => x.Schedules)
                .HasForeignKey(x => x.ConfigurationId);
        }
    }
}