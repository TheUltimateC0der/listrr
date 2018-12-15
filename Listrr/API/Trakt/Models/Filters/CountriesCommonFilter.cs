using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using Refit;

namespace Listrr.API.Trakt.Models.Filters
{
    public class CountriesCommonFilter
    {

        public CountriesCommonFilter()
        {
            
        }

        public CountriesCommonFilter(string csv)
        {
            Languages = csv.Split(",");
        }


        [AliasAs("languages")]
        public string[] Languages { get; set; }

    }
}