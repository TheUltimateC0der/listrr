using Newtonsoft.Json;

namespace Listrr.API.GitHub
{
    public class Viewer
    {
        [JsonProperty("sponsorshipsAsMaintainer")]
        public SponsorshipsAsMaintainer SponsorshipsAsMaintainer { get; set; }
    }
}