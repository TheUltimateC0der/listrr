using Refit;

using System.Collections.Generic;
using System.Linq;

namespace Listrr.Data.Trakt.Filters
{
    public class NetworksShowFilter
    {

        public NetworksShowFilter()
        {

        }

        public NetworksShowFilter(IEnumerable<string> networks)
        {
            this.Networks = networks?.ToArray();
        }

        [AliasAs("networks")]
        public string[] Networks { get; set; }

    }
}