using System.Collections.Generic;
using System.Linq;
using Refit;

namespace Listrr.API.Trakt.Models.Filters
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