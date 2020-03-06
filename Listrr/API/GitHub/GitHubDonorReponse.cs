using Newtonsoft.Json;

namespace Listrr.API.GitHub
{
    public class GitHubDonorResponse
    {
        [JsonProperty("viewer")]
        public Viewer Viewer { get; set; }
    }
}