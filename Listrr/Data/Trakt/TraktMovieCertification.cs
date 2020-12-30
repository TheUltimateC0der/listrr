using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Listrr.Data.Trakt
{
    public class TraktMovieCertification
    {
        public string Slug { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }
    }

    public class TraktMovieCertificationConfiguration : IEntityTypeConfiguration<TraktMovieCertification>
    {
        public void Configure(EntityTypeBuilder<TraktMovieCertification> builder)
        {
            builder
                .HasIndex(x => x.Slug)
                .IsUnique();

            builder
                .HasKey(x => x.Slug);
        }
    }
}
