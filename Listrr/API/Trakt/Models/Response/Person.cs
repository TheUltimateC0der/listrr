using Newtonsoft.Json;

namespace Listrr.API.Trakt.Models
{
    public class Person
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ids")]
        public MediaIds Ids { get; set; }

    }
}