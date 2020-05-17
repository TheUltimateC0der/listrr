namespace Listrr.Configuration
{
    public class TraktAPIConfiguration
    {
        public int DelaySearch { get; set; }

        public int DelayList { get; set; }

        public int DelayIdSearch { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public uint? FetchLimitSearch { get; set; }

        public uint? FetchLimitList { get; set; }

        public int ChunkBy { get; set; }

        public int DelayRequeue { get; set; }
    }
}