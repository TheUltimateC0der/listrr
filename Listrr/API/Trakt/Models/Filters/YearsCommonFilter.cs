using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;

namespace Listrr.API.Trakt.Models.Filters
{
    public class YearsCommonFilter
    {

        public YearsCommonFilter(int From, int To)
        {
            Years = $"{From}-{To}";
        }

        [AliasAs("ratings")]
        public string Years { get; set; }

    }
}