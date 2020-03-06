using Newtonsoft.Json;

namespace Listrr.API.GitHub
{
    public class SponsorshipsAsMaintainer
    {
        [JsonProperty("nodes")]
        public Node[] Nodes { get; set; }
    }
}