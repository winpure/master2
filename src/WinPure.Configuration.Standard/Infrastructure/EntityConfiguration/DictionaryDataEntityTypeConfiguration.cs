using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WinPure.Configuration.Infrastructure.EntityConfiguration
{
    public class DictionaryDataEntityTypeConfiguration : IEntityTypeConfiguration<DictionaryDataEntity>
    {
        public void Configure(EntityTypeBuilder<DictionaryDataEntity> builder)
        {
            builder.ToTable("DictionaryData");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.SearchValue)
                .IsRequired();

            builder.Property(e => e.DictionaryId).IsRequired();

            builder.HasOne(o => o.Dictionary)
                .WithMany(o => o.DictionaryData)
                .HasForeignKey(o => o.DictionaryId);
        }
    }
}