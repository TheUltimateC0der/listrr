using Refit;

using System.Collections.Generic;
using System.Linq;

namespace Listrr.Data.Trakt.Filters
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