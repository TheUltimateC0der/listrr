using Newtonsoft.Json;

namespace Listrr.API.Trakt.Models
{
    public class Season
    {

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("ids")]
        public MediaIds Ids { get; set; }

    }
}