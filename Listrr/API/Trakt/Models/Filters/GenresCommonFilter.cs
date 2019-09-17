using System.Collections.Generic;
using System.Linq;
using Refit;

namespace Listrr.API.Trakt.Models.Filters
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