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
    public class GetMovieGenresRecurringJob : IRecurringJob
    {
        private readonly TraktClient _traktClient;
        private readonly TraktAPIConfiguration _traktApiConfiguration;
        private readonly ITraktMovieRepository _traktMovieRepository;

        public GetMovieGenresRecurringJob(TraktAPIConfiguration traktApiConfiguration, ITraktMovieRepository traktMovieRepository)
        {
            _traktApiConfiguration = traktApiConfiguration;
            _traktMovieRepository = traktMovieRepository;

            _traktClient = new TraktClient(_traktApiConfiguration.ClientId, _traktApiConfiguration.ClientSecret);
        }


        public async Task Execute()
        {
            var result = await _traktClient.Genres.GetMovieGenresAsync();

            if (result.IsSuccess)
            {
                var currentGenres = await _traktMovieRepository.GetGenres();

                foreach (var traktGenre in result.Value)
                {
                    var currentGenre = currentGenres.FirstOrDefault(x => x.Slug == traktGenre.Slug);

                    if (currentGenre == null)
                    {
                        await _traktMovieRepository.CreateGenre(new TraktMovieGenre()
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