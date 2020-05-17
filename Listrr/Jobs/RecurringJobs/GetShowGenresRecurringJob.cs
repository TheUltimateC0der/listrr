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
    public class GetShowGenresRecurringJob : IRecurringJob
    {
        private readonly TraktClient _traktClient;
        private readonly TraktAPIConfiguration _traktApiConfiguration;
        private readonly ITraktShowRepository _traktShowRepository;

        public GetShowGenresRecurringJob(TraktAPIConfiguration traktApiConfiguration, ITraktShowRepository traktShowRepository)
        {
            _traktApiConfiguration = traktApiConfiguration;
            _traktShowRepository = traktShowRepository;

            _traktClient = new TraktClient(_traktApiConfiguration.ClientId, _traktApiConfiguration.ClientSecret);
        }


        public async Task Execute()
        {
            var result = await _traktClient.Genres.GetShowGenresAsync();

            if (result.IsSuccess)
            {
                var currentGenres = await _traktShowRepository.GetGenres();

                foreach (var traktGenre in result.Value)
                {
                    var currentGenre = currentGenres.FirstOrDefault(x => x.Slug != traktGenre.Slug);

                    if (currentGenre == null)
                    {
                        await _traktShowRepository.CreateGenre(new TraktShowGenre()
                        {
                            Name = traktGenre.Name,
                            Slug = traktGenre.Slug
                        });
                    }
                }
            }
        }
    }
}