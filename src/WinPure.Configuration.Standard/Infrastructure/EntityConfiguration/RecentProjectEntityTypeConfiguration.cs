using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WinPure.Configuration.Infrastructure.EntityConfiguration
{
    public class RecentProjectEntityTypeConfiguration : IEntityTypeConfiguration<RecentProjectEntity>
    {
        public void Configure(EntityTypeBuilder<RecentProjectEntity> builder)
        {
            builder.ToTable("RecentProjects");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.ProjectName).IsRequired();
            builder.Property(e => e.ProjectPath).IsRequired();
            builder.Property(e => e.ModifiedBy).IsRequired(false);
            builder.Property(e => e.CreateDate).HasDefaultValueSql("datetime('now', 'localtime')");
            builder.Property(e => e.LastUpdateDate).HasDefaultValueSql("datetime('now', 'localtime')");
        }
    }
}