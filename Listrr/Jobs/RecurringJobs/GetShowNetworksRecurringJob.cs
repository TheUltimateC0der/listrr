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
    public class GetShowNetworksRecurringJob : IRecurringJob
    {
        private readonly TraktClient _traktClient;
        private readonly TraktAPIConfiguration _traktApiConfiguration;
        private readonly ITraktShowRepository _traktShowRepository;

        public GetShowNetworksRecurringJob(TraktAPIConfiguration traktApiConfiguration, ITraktShowRepository traktShowRepository)
        {
            _traktApiConfiguration = traktApiConfiguration;
            _traktShowRepository = traktShowRepository;

            _traktClient = new TraktClient(_traktApiConfiguration.ClientId, _traktApiConfiguration.ClientSecret);
        }


        public async Task Execute()
        {
            var result = await _traktClient.Networks.GetNetworksAsync();

            if (result.IsSuccess)
            {
                var currentNetworks = await _traktShowRepository.GetNetworks();

                foreach (var traktNetwork in result.Value)
                {
                    if (currentNetworks.All(x => x.Name != traktNetwork.Name))
                    {
                        await _traktShowRepository.CreateNetwork(new TraktShowNetwork()
                        {
                            Name = traktNetwork.Name
                        });
                    }
                }
            }
        }
    }
}