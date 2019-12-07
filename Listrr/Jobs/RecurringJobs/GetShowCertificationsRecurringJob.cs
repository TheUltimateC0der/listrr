using Listrr.Configuration;
using Listrr.Data;
using Listrr.Data.Trakt;

using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Threading.Tasks;

using TraktNet;

namespace Listrr.Jobs.RecurringJobs
{
    public class GetShowCertificationsRecurringJob : IRecurringJob
    {

        private readonly TraktClient _traktClient;
        private readonly AppDbContext _appDbContext;
        private readonly TraktAPIConfiguration _traktApiConfiguration;

        public GetShowCertificationsRecurringJob(AppDbContext appDbContext, TraktAPIConfiguration traktApiConfiguration)
        {
            _appDbContext = appDbContext;
            _traktApiConfiguration = traktApiConfiguration;

            _traktClient = new TraktClient(_traktApiConfiguration.ClientId, _traktApiConfiguration.ClientSecret);
        }


        public async Task Execute()
        {
            var result = await _traktClient.Certifications.GetShowCertificationsAsync();

            if (result.IsSuccess)
            {
                var currentCertifications = await _appDbContext.TraktShowCertifications.ToListAsync();

                foreach (var traktCertification in result.Value.US)
                {
                    if (currentCertifications.All(x => x.Slug != traktCertification.Slug))
                    {
                        await _appDbContext.TraktShowCertifications.AddAsync(new TraktShowCertification()
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