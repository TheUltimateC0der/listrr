using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;

namespace Listrr.API.Trakt.Models.Filters
{
    public class LanguagesCommonFilter
    {

        public LanguagesCommonFilter()
        {
            
        }

        public LanguagesCommonFilter(string csv)
        {
            Languages = csv.Split(",");
        }


        [AliasAs("languages")]
        public string[] Languages { get; set; }

    }
}