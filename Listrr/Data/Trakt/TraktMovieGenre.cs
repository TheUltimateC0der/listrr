using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Listrr.Data.Trakt
{
    public class TraktMovieGenre
    {
        public string Name { get; set; }

        public string Slug { get; set; }
    }

    public class TraktMovieGenreConfiguration : IEntityTypeConfiguration<TraktMovieGenre>
    {
        public void Configure(EntityTypeBuilder<TraktMovieGenre> builder)
        {
            builder
                .HasIndex(x => x.Slug)
                .IsUnique();

            builder
                .HasKey(x => x.Slug);
        }
    }
}