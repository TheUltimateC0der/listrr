using Newtonsoft.Json;

namespace Listrr.API.Trakt.Models
{
    public class MediaIds
    {

        [JsonProperty("trakt")]
        public int Trakt { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("tvdb")]
        public int TVDB { get; set; }

        [JsonProperty("imdb")]
        public string IMDB { get; set; }

        [JsonProperty("tvdb")]
        public int TMDB { get; set; }

    }
}