using System.Linq;
using System.Threading.Tasks;

using Hangfire;

using Listrr.Configuration;
using Listrr.Data.Trakt;
using Listrr.Repositories;

using TraktNet;

namespace Listrr.Jobs.RecurringJobs
{

    [Queue("system")]
    public class GetShowCertificationsRecurringJob : IRecurringJob
    {

        private readonly TraktClient _traktClient;
        private readonly TraktAPIConfiguration _traktApiConfiguration;
        private readonly ITraktShowRepository _traktShowRepository;

        public GetShowCertificationsRecurringJob(TraktAPIConfiguration traktApiConfiguration, ITraktShowRepository traktShowRepository)
        {
            _traktApiConfiguration = traktApiConfiguration;
            _traktShowRepository = traktShowRepository;

            _traktClient = new TraktClient(_traktApiConfiguration.ClientId, _traktApiConfiguration.ClientSecret);
        }


        public async Task Execute()
        {
            var result = await _traktClient.Certifications.GetShowCertificationsAsync();

            if (result.IsSuccess)
            {
                var currentCertifications = await _traktShowRepository.GetCertifications();

                foreach (var traktCertification in result.Value.US)
                {
                    var currentCertification = currentCertifications.FirstOrDefault(x => x.Slug != traktCertification.Slug);

                    if (currentCertification == null)
                    {
                        await _traktShowRepository.CreateCertification(new TraktShowCertification()
                        {
                            Name = traktCertification.Name,
                            Description = traktCertification.Description,
                            Slug = traktCertification.Slug
                        });
                    }
                }
            }
        }
    }
}