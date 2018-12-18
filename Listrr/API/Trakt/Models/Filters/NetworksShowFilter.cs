using System.Collections.Generic;
using System.Linq;
using Refit;

namespace Listrr.API.Trakt.Models.Filters
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