using Listrr.Data;

namespace Listrr.Configuration
{
    public class LimitConfiguration : ILimitConfiguration
    {
        public int ListLimit { get; set; }
        public bool ExclusionFilters { get; set; }
        public bool UpdateAfterEdit { get; set; }
        public bool UpdateManual { get; set; }
        public string QueueName { get; set; }
        public UserLevel Level { get; set; }
    }
}