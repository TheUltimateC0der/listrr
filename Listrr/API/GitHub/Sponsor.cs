using Newtonsoft.Json;

namespace Listrr.API.GitHub
{
    public class Sponsor
    {
        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("databaseId")]
        public long DatabaseId { get; set; }
    }
}