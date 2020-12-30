using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Listrr.Data
{
    public class CountryCode
    {
        public string Name { get; set; }

        public string Code { get; set; }
    }

    public class CountryCodeConfiguration : IEntityTypeConfiguration<CountryCode>
    {
        public void Configure(EntityTypeBuilder<CountryCode> builder)
        {
            builder
                .HasIndex(x => x.Code);

            builder
                .HasKey(x => new { x.Name, x.Code });
        }
    }
}