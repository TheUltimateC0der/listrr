using Newtonsoft.Json;

namespace Listrr.API.Trakt.Models
{
    public class Episode
    {
        [JsonProperty("season")]
        public int Season { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("ids")]
        public MediaIds Ids { get; set; }

    }
}
