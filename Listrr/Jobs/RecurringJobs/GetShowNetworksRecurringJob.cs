using System.Linq;
using System.Threading.Tasks;
using Listrr.Data;
using Listrr.Data.Trakt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TraktNet;

namespace Listrr.BackgroundJob
{
    public class GetShowNetworksRecurringJob : IRecurringJob
    {

        private readonly TraktClient traktClient;
        private readonly IConfiguration configuration;
        private readonly AppDbContext appDbContext;

        public GetShowNetworksRecurringJob(AppDbContext appDbContext, IConfiguration configuration)
        {
            this.configuration = configuration;
            this.appDbContext = appDbContext;

            traktClient = new TraktClient(configuration["Trakt:ClientID"], configuration["Trakt:ClientSecret"]);
        }


        public async Task Execute()
        {
            var result = await traktClient.Networks.GetNetworksAsync();

            if (result.IsSuccess)
            {
                var currentNetworks = await appDbContext.TraktShowNetworks.ToListAsync();

                foreach (var traktNetwork in result.Value)
                {
                    if (currentNetworks.All(x => x.Name != traktNetwork.Name))
                    {
                        await appDbContext.TraktShowNetworks.AddAsync(new TraktShowNetwork()
                        {
                            Name = traktNetwork.Name
                        });
                        await appDbContext.SaveChangesAsync();
                    }
                }
            }
        }
    }
}