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
    public class GetMovieCertificationsRecurringJob : IRecurringJob
    {

        private readonly TraktClient traktClient;
        private readonly IConfiguration configuration;
        private readonly AppDbContext appDbContext;

        public GetMovieCertificationsRecurringJob(AppDbContext appDbContext, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.appDbContext = appDbContext;

            this.traktClient = new TraktClient(configuration["Trakt:ClientID"], configuration["Trakt:ClientSecret"]);
        }


        public async Task Execute()
        {
            var result = await traktClient.Certifications.GetMovieCertificationsAsync();

            if (result.IsSuccess)
            {
                var currentCertifications = await appDbContext.TraktMovieCertifications.ToListAsync();

                foreach (var traktCertification in result.Value.US)
                {
                    if (currentCertifications.All(x => x.Slug != traktCertification.Slug))
                    {
                        await appDbContext.TraktMovieCertifications.AddAsync(new TraktMovieCertification()
                        {
                            Name = traktCertification.Name,
                            Description = traktCertification.Description,
                            Slug = traktCertification.Slug
                        });
                        await appDbContext.SaveChangesAsync();
                    }
                }
            }
        }
    }
}