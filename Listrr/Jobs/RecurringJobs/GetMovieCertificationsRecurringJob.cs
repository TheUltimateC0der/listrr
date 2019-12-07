using Listrr.Configuration;
using Listrr.Data;
using Listrr.Data.Trakt;

using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Threading.Tasks;

using TraktNet;

namespace Listrr.Jobs.RecurringJobs
{
    public class GetMovieCertificationsRecurringJob : IRecurringJob
    {

        private readonly TraktClient _traktClient;
        private readonly AppDbContext _appDbContext;
        private readonly TraktAPIConfiguration _traktApiConfiguration;

        public GetMovieCertificationsRecurringJob(AppDbContext appDbContext, TraktAPIConfiguration traktApiConfiguration)
        {
            _appDbContext = appDbContext;
            _traktApiConfiguration = traktApiConfiguration;

            _traktClient = new TraktClient(_traktApiConfiguration.ClientId, _traktApiConfiguration.ClientSecret);
        }


        public async Task Execute()
        {
            var result = await _traktClient.Certifications.GetMovieCertificationsAsync();

            if (result.IsSuccess)
            {
                var currentCertifications = await _appDbContext.TraktMovieCertifications.ToListAsync();

                foreach (var traktCertification in result.Value.US)
                {
                    if (currentCertifications.All(x => x.Slug != traktCertification.Slug))
                    {
                        await _appDbContext.TraktMovieCertifications.AddAsync(new TraktMovieCertification()
                        {
                            Name = traktCertification.Name,
                            Description = traktCertification.Description,
                            Slug = traktCertification.Slug
                        });
                        await _appDbContext.SaveChangesAsync();
                    }
                }
            }
        }
    }
}