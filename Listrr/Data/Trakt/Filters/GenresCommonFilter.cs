using Refit;

using System.Collections.Generic;
using System.Linq;

namespace Listrr.Data.Trakt.Filters
{
    public class GenresCommonFilter
    {

        public GenresCommonFilter()
        {

        }

        public GenresCommonFilter(IEnumerable<string> genres)
        {
            this.Genres = genres?.ToArray();
        }

        [AliasAs("genres")]
        public string[] Genres { get; set; }

    }
}