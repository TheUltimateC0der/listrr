using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Newtonsoft.Json;

using System.ComponentModel.DataAnnotations.Schema;

namespace Listrr.Data.Trakt
{
    public class TraktShowStatus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class TraktShowStatusConfiguration : IEntityTypeConfiguration<TraktShowStatus>
    {
        public void Configure(EntityTypeBuilder<TraktShowStatus> builder)
        {
            builder
                .HasIndex(x => x.Name)
                .IsUnique();

            builder
                .HasKey(x => x.Name);
        }
    }
}