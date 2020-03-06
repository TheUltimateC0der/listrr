using Newtonsoft.Json;

namespace Listrr.API.GitHub
{
    public class Node
    {
        [JsonProperty("sponsor")]
        public Sponsor Sponsor { get; set; }

        [JsonProperty("tier")]
        public Tier Tier { get; set; }
    }
}