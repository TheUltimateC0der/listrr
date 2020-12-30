using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Newtonsoft.Json;

using System.ComponentModel.DataAnnotations.Schema;

namespace Listrr.Data.Trakt
{
    public class TraktShowNetwork
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonIgnore]
        public string Id { get; set; }


        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class TraktShowNetworkConfiguration : IEntityTypeConfiguration<TraktShowNetwork>
    {
        public void Configure(EntityTypeBuilder<TraktShowNetwork> builder)
        {

        }
    }
}