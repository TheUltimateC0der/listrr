using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Listrr.Data
{
    public class LanguageCode
    {
        public string Name { get; set; }

        public string Code { get; set; }

        public string Description => $"{Code} ({Name})";
    }

    public class LanguageCodeConfiguration : IEntityTypeConfiguration<LanguageCode>
    {
        public void Configure(EntityTypeBuilder<LanguageCode> builder)
        {
            builder
                .HasIndex(x => x.Code);

            builder
                .HasKey(x => new { x.Name, x.Code });
        }
    }
}