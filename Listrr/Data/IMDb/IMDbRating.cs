using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Listrr.Data.IMDb
{
    public class IMDbRating : CreatedAndUpdated
    {
        public string IMDbId { get; set; }

        public float Rating { get; set; }

        public int Votes { get; set; }

    }

    public class IMDbRatingConfiguration : IEntityTypeConfiguration<IMDbRating>
    {
        public void Configure(EntityTypeBuilder<IMDbRating> builder)
        {
            builder
                .HasIndex(x => x.IMDbId)
                .IsUnique();

            builder
                .HasKey(x => x.IMDbId);
        }
    }
}