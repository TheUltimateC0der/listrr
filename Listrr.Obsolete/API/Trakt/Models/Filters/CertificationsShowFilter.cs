using System.Collections.Generic;
using System.Linq;
using Refit;

namespace Listrr.API.Trakt.Models.Filters
{
    public class CertificationsShowFilter
    {
        public CertificationsShowFilter()
        {
            
        }

        public CertificationsShowFilter(IEnumerable<string> certifications)
        {
            this.Certifications = certifications?.ToArray();
        }


        [AliasAs("certifications")]
        public string[] Certifications { get; set; }

    }
}
