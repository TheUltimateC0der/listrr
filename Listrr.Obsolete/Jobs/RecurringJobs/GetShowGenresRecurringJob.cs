using Listrr.Configuration;
using Listrr.Data;
using Listrr.Data.Trakt;

using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Threading.Tasks;

using TraktNet;

namespace Listrr.Jobs.RecurringJobs
{
    public class GetShowGenresRecurringJob : IRecurringJob
    {

        private readonly TraktClient _traktClient;
        private readonly AppDbContext _appDbContext;
        private readonly TraktAPIConfiguration _traktApiConfiguration;

        public GetShowGenresRecurringJob(AppDbContext appDbContext, TraktAPIConfiguration traktApiConfiguration)
        {
            _appDbContext = appDbContext;
            _traktApiConfiguration = traktApiConfiguration;

            _traktClient = new TraktClient(_traktApiConfiguration.ClientId, _traktApiConfiguration.ClientSecret);
        }


        public async Task Execute()
        {
            var result = await _traktClient.Genres.GetShowGenresAsync();

            if (result.IsSuccess)
            {
                var currentGenres = await _appDbContext.TraktShowGenres.ToListAsync();

                foreach (var traktGenre in result.Value)
                {
                    if (currentGenres.All(x => x.Slug != traktGenre.Slug))
                    {
                        await _appDbContext.TraktShowGenres.AddAsync(new TraktShowGenre()
                        {
                            Name = traktGenre.Name,
                            Slug = traktGenre.Slug
                        });
                        await _appDbContext.SaveChangesAsync();
                    }
                }
            }
        }
    }
}