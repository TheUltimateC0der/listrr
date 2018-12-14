using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;

namespace Listrr.API.Trakt.Models.Filters
{
    public class RuntimesCommonFilter
    {

        public RuntimesCommonFilter(int From, int To)
        {
            Runtimes = $"{From}-{To}";
        }

        [AliasAs("runtimes")]
        public string Runtimes { get; set; }

    }
}