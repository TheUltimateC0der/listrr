using System.Security.Cryptography;

namespace Listrr.Configuration
{
    public class TraktAPIConfiguration
    {

        public int DelaySearch { get; set; }

        public int DelayList { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public uint? FetchLimitSearch { get; set; }

        public uint? FetchLimitList { get; set; }
        
    }
}