using Refit;

using System.Collections.Generic;
using System.Linq;

namespace Listrr.Data.Trakt.Filters
{
    public class LanguagesCommonFilter
    {

        public LanguagesCommonFilter()
        {

        }

        public LanguagesCommonFilter(IEnumerable<string> languages)
        {
            this.Languages = languages?.ToArray();
        }


        [AliasAs("languages")]
        public string[] Languages { get; set; }

    }
}