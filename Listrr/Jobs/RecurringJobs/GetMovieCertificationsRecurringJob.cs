using System.Linq;
using System.Threading.Tasks;

using Hangfire;

using Listrr.Configuration;
using Listrr.Data;
using Listrr.Data.Trakt;
using Listrr.Repositories;
using Microsoft.EntityFrameworkCore;

using TraktNet;

namespace Listrr.Jobs.RecurringJobs
{

    [Queue("system")]
    public class GetMovieCertificationsRecurringJob : IRecurringJob
    {
        private readonly TraktClient _traktClient;
        private readonly TraktAPIConfiguration _traktApiConfiguration;
        private readonly ITraktMovieRepository _traktMovieRepository;

        public GetMovieCertificationsRecurringJob(TraktAPIConfiguration traktApiConfiguration, ITraktMovieRepository traktMovieRepository)
        {
            _traktApiConfiguration = traktApiConfiguration;
            _traktMovieRepository = traktMovieRepository;

            _traktClient = new TraktClient(_traktApiConfiguration.ClientId, _traktApiConfiguration.ClientSecret);
        }

        public async Task Execute()
        {
            var result = await _traktClient.Certifications.GetMovieCertificationsAsync();

            if (result.IsSuccess)
            {
                var currentCertifications = await _traktMovieRepository.GetCertifications();

                foreach (var traktCertification in result.Value.US)
                {
                    var certification = currentCertifications.FirstOrDefault(x => x.Slug == traktCertification.Slug);

                    if (certification == null)
                    {
                        await _traktMovieRepository.CreateCertification(new TraktMovieCertification()
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