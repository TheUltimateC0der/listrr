using Refit;

using System.Collections.Generic;
using System.Linq;

namespace Listrr.Data.Trakt.Filters
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