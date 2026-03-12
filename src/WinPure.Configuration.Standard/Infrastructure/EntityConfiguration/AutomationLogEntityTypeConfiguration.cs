using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WinPure.Configuration.Infrastructure.EntityConfiguration
{
    public class AutomationLogEntityTypeConfiguration : IEntityTypeConfiguration<AutomationLogEntity>
    {
        public void Configure(EntityTypeBuilder<AutomationLogEntity> builder)
        {
            builder.ToTable("AutomationLogs");

            builder.HasKey(o => o.Id);

            builder.Property(x => x.DateOfRun).IsRequired();
            builder.Property(x => x.Successful).IsRequired();

            builder.HasOne(x => x.Schedule)
                .WithMany(x => x.Logs)
                .HasForeignKey(x => x.ScheduleId);
        }
    }
}