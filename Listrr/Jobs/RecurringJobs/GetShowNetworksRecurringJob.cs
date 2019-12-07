using Listrr.Configuration;
using Listrr.Data;
using Listrr.Data.Trakt;

using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Threading.Tasks;

using TraktNet;

namespace Listrr.Jobs.RecurringJobs
{
    public class GetShowNetworksRecurringJob : IRecurringJob
    {

        private readonly TraktClient _traktClient;
        private readonly AppDbContext _appDbContext;
        private readonly TraktAPIConfiguration _traktApiConfiguration;

        public GetShowNetworksRecurringJob(AppDbContext appDbContext, TraktAPIConfiguration traktApiConfiguration)
        {
            _appDbContext = appDbContext;
            _traktApiConfiguration = traktApiConfiguration;

            _traktClient = new TraktClient(_traktApiConfiguration.ClientId, _traktApiConfiguration.ClientSecret);
        }


        public async Task Execute()
        {
            var result = await _traktClient.Networks.GetNetworksAsync();

            if (result.IsSuccess)
            {
                var currentNetworks = await _appDbContext.TraktShowNetworks.ToListAsync();

                foreach (var traktNetwork in result.Value)
                {
                    if (currentNetworks.All(x => x.Name != traktNetwork.Name))
                    {
                        await _appDbContext.TraktShowNetworks.AddAsync(new TraktShowNetwork()
                        {
                            Name = traktNetwork.Name
                        });
                        await _appDbContext.SaveChangesAsync();
                    }
                }
            }
        }
    }
}