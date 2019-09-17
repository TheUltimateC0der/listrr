using System.Linq;
using System.Threading.Tasks;
using Listrr.Data;
using Listrr.Data.Trakt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TraktNet;

namespace Listrr.BackgroundJob
{
    public class GetShowGenresRecurringJob : IRecurringJob
    {

        private readonly TraktClient traktClient;
        private readonly IConfiguration configuration;
        private readonly AppDbContext appDbContext;

        public GetShowGenresRecurringJob(AppDbContext appDbContext, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.appDbContext = appDbContext;

            traktClient = new TraktClient(configuration["Trakt:ClientID"], configuration["Trakt:ClientSecret"]);
        }


        public async Task Execute()
        {
            var result = await traktClient.Genres.GetShowGenresAsync();

            if (result.IsSuccess)
            {
                var currentGenres = await appDbContext.TraktShowGenres.ToListAsync();

                foreach (var traktGenre in result.Value)
                {
                    if (currentGenres.All(x => x.Slug != traktGenre.Slug))
                    {
                        await appDbContext.TraktShowGenres.AddAsync(new TraktShowGenre()
                        {
                            Name = traktGenre.Name,
                            Slug = traktGenre.Slug
                        });
                        await appDbContext.SaveChangesAsync();
                    }
                }
            }
        }
    }
}