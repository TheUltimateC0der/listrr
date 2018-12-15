using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;

namespace Listrr.API.Trakt.Models.Filters
{
    public class GenresCommonFilter
    {

        public GenresCommonFilter()
        {
            
        }

        public GenresCommonFilter(string csv)
        {
            Genres = csv.Split(",");
        }


        [AliasAs("genres")]
        public string[] Genres { get; set; }

    }
}