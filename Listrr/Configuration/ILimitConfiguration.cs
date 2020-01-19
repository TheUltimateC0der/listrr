using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Listrr.Data;

namespace Listrr.Configuration
{
    public interface ILimitConfiguration
    {

        public int ListLimit { get; set; }

        public bool ExclusionFilters { get; set; }

        public bool UpdateAfterEdit { get; set; }

        public bool UpdateManual { get; set; }

        public string QueueName { get; set; }

        public UserLevel Level { get; set; }

    }
}