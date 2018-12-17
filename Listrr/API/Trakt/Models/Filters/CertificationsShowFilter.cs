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

        public CertificationsShowFilter(string csv)
        {
            if (csv.Length > 0 || csv.Contains(","))
            {
                Certifications = csv.Split(",");
            }
            else
            {
                Certifications = null;
            }
        }


        [AliasAs("certifications")]
        public string[] Certifications { get; set; }

    }
}
