using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WinPure.Configuration.Infrastructure.EntityConfiguration
{
    public class DictionaryNameEntityTypeConfiguration : IEntityTypeConfiguration<DictionaryNameEntity>
    {
        public void Configure(EntityTypeBuilder<DictionaryNameEntity> builder)
        {
            builder.ToTable("DictionaryName");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.DictionaryName)
                .IsRequired();
        }
    }
}