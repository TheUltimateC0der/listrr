using System.Collections.Generic;
using System.Linq;
using Refit;

namespace Listrr.API.Trakt.Models.Filters
{
    public class TranslationsBasicFilter
    {

        public TranslationsBasicFilter()
        {
            
        }

        public TranslationsBasicFilter(IEnumerable<string> translations)
        {
            this.Translations = translations?.ToArray();
        }


        [AliasAs("translations")]
        public string[] Translations { get; set; }

    }
}