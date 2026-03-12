using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WinPure.Configuration.Infrastructure.EntityConfiguration;

public class ConnectionSettingEntityTypeConfiguration : IEntityTypeConfiguration<ConnectionSettingEntity>
{
    public void Configure(EntityTypeBuilder<ConnectionSettingEntity> builder)
    {
        builder.ToTable("ConnectionSettings");

        builder.HasKey(e => e.Id);
    }
}