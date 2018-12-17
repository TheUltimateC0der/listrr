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
            if (csv.Length > 0 || csv.Contains(","))
            {
                Languages = csv.Split(",");
            }
            else
            {
                Languages = null;
            }
        }


        [AliasAs("languages")]
        public string[] Languages { get; set; }

    }
}