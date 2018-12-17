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
            if (csv.Length > 0 || csv.Contains(","))
            {
                Genres = csv.Split(",");
            }
            else
            {
                Genres = null;
            }
        }


        [AliasAs("genres")]
        public string[] Genres { get; set; }

    }
}