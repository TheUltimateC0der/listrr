using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;

namespace Listrr.API.Trakt.Models.Filters
{
    public class RatingsCommonFilter
    {

        public RatingsCommonFilter(int From, int To)
        {
            Ratings = $"{From}-{To}";
        }

        [AliasAs("ratings")]
        public string Ratings { get; set; }

    }
}