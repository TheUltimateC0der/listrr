using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Listrr.Data.Trakt
{
    public class TraktShowCertification
    {
        public string Slug { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }
    }

    public class TraktShowCertificationConfiguration : IEntityTypeConfiguration<TraktShowCertification>
    {
        public void Configure(EntityTypeBuilder<TraktShowCertification> builder)
        {
            builder
                .HasIndex(x => x.Slug)
                .IsUnique();

            builder
                .HasKey(x => x.Slug);
        }
    }
}
