using Refit;

using System.Collections.Generic;
using System.Linq;

namespace Listrr.Data.Trakt.Filters
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
