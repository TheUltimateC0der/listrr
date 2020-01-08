using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

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
}