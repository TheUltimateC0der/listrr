using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Listrr.Data.Trakt
{
    public class TraktShowStatus
    {
        
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [JsonProperty("name")]
        public string Name { get; set; }

    }
}