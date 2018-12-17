using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;

namespace Listrr.API.Trakt.Models.Filters
{
    public class TranslationsBasicFilter
    {

        public TranslationsBasicFilter()
        {
            
        }

        public TranslationsBasicFilter(string csv)
        {
            if (csv.Length > 0 || csv.Contains(","))
            {
                Translations = csv.Split(",");
            }
            else
            {
                Translations = null;
            }
        }


        [AliasAs("translations")]
        public string[] Translations { get; set; }

    }
}