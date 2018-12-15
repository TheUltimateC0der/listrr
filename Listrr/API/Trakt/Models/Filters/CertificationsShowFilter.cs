using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;

namespace Listrr.API.Trakt.Models.Filters
{
    public class CertificationsShowFilter
    {
        public CertificationsShowFilter()
        {
            
        }


        [AliasAs("certifications")]
        public string[] Certifications { get; set; }

    }
}
