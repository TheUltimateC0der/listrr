using System.Collections.Generic;
using System.Linq;
using Refit;

namespace Listrr.API.Trakt.Models.Filters
{
    public class CertificationsMovieFilter
    {

        public CertificationsMovieFilter()
        {
            
        }

        public CertificationsMovieFilter(IEnumerable<string> certifications)
        {
            this.Certifications = certifications?.ToArray();
        }


        [AliasAs("certifications")]
        public string[] Certifications { get; set; }

    }
}