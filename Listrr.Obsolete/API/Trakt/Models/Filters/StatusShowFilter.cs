using System.Collections.Generic;
using System.Linq;
using Refit;

namespace Listrr.API.Trakt.Models.Filters
{
    public class StatusShowFilter
    {

        public StatusShowFilter()
        {
            
        }

        public StatusShowFilter(IEnumerable<string> status)
        {
            this.Status = status?.ToArray();
        }

        [AliasAs("status")]
        public string[] Status { get; set; }

    }
}