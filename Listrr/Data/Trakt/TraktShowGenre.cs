
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Listrr.Data.Trakt
{
    public class TraktShowGenre
    {
        public string Name { get; set; }

        public string Slug { get; set; }
    }

    public class TraktShowGenreConfiguration : IEntityTypeConfiguration<TraktShowGenre>
    {
        public void Configure(EntityTypeBuilder<TraktShowGenre> builder)
        {
            builder
                .HasIndex(x => x.Slug)
                .IsUnique();

            builder
                .HasKey(x => x.Slug);
        }
    }
}