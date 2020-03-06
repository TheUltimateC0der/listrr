using Newtonsoft.Json;

namespace Listrr.API.GitHub
{
    public class Tier
    {
        [JsonProperty("monthlyPriceInDollars")]
        public long MonthlyPriceInDollars { get; set; }
    }
}