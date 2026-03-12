using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WinPure.Configuration.Infrastructure.EntityConfiguration
{
    public class FavouriteSourceEntityEntityTypeConfiguration : IEntityTypeConfiguration<FavouriteSourceEntity>
    {
        public void Configure(EntityTypeBuilder<FavouriteSourceEntity> builder)
        {
            builder.ToTable("FavouriteSources");

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Source).IsRequired(true);
        }
    }
}