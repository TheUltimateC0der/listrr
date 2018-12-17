using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Listrr.Data;
using Listrr.Data.Trakt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TraktNet;

namespace Listrr.BackgroundJob
{
    public class GetMovieGenresRecurringJob : IRecurringJob
    {

        private readonly TraktClient traktClient;
        private readonly IConfiguration configuration;
        private readonly AppDbContext appDbContext;

        public GetMovieGenresRecurringJob(AppDbContext appDbContext, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.appDbContext = appDbContext;

            traktClient = new TraktClient(configuration["Trakt:ClientID"], configuration["Trakt:ClientSecret"]);
        }


        public async Task Execute()
        {
            var result = await traktClient.Genres.GetMovieGenresAsync();

            if (result.IsSuccess)
            {
                var currentGenres = await appDbContext.TraktMovieGenres.ToListAsync();

                foreach (var traktGenre in result.Value)
                {
                    if (currentGenres.All(x => x.Slug != traktGenre.Slug))
                    {
                        await appDbContext.TraktMovieGenres.AddAsync(new TraktMovieGenre()
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