using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WinPure.Configuration.Infrastructure.EntityConfiguration
{
    public class ProperCaseSettingEntityTypeConfiguration : IEntityTypeConfiguration<ProperCaseSettingEntity>
    {
        public void Configure(EntityTypeBuilder<ProperCaseSettingEntity> builder)
        {
            builder.ToTable("ProperCaseSettings");

            builder.HasKey(o => o.Name);
            builder.Property(x => x.Value).IsRequired();
        }
    }
}