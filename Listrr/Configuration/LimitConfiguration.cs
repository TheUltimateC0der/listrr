using Listrr.Data;

namespace Listrr.Configuration
{
    public class LimitConfiguration : ILimitConfiguration
    {
        public int ListLimit { get; set; }
        public int Amount { get; set; }
        public bool ExclusionFilters { get; set; }
        public bool UpdateAfterEdit { get; set; }
        public bool UpdateManual { get; set; }
        public bool ListsFromNames { get; set; }
        public string QueueName { get; set; }
        public UserLevel Level { get; set; }
        public bool IMDbRatings { get; set; }
    }
}