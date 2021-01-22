using Refit;

using System.Collections.Generic;
using System.Linq;

namespace Listrr.Data.Trakt.Filters
{
    public class CountriesCommonFilter
    {

        public CountriesCommonFilter()
        {

        }

        public CountriesCommonFilter(IEnumerable<string> countries)
        {
            this.Languages = countries?.ToArray();
        }


        [AliasAs("languages")]
        public string[] Languages { get; set; }

    }
}