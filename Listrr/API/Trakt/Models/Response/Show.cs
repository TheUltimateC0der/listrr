using Newtonsoft.Json;

namespace Listrr.API.Trakt.Models
{
    public class Show
    {

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("ids")]
        public MediaIds Ids { get; set; }

    }
}
