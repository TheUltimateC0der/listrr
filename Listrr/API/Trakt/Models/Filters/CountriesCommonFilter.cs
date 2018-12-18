using System.Collections.Generic;
using System.Linq;
using Refit;

namespace Listrr.API.Trakt.Models.Filters
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