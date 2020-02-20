using Hangfire;

using Listrr.Configuration;
using Listrr.Data;
using Listrr.Repositories;
using Listrr.Services;

using System.Linq;
using System.Threading.Tasks;

namespace Listrr.Jobs.RecurringJobs
{

    [Queue("system")]
    public class ProcessDonorListsRecurringJob : IRecurringJob
    {
        private readonly ITraktListRepository _traktRepository;
        private readonly LimitConfigurationList _limitConfigurationList;
        private readonly IBackgroundJobQueueService _backgroundJobQueueService;

        public ProcessDonorListsRecurringJob(LimitConfigurationList limitConfigurationList, IBackgroundJobQueueService backgroundJobQueueService, ITraktListRepository traktRepository)
        {
            _limitConfigurationList = limitConfigurationList;
            _backgroundJobQueueService = backgroundJobQueueService;
            _traktRepository = traktRepository;
        }


        public async Task Execute()
        {
            foreach (var limitConfiguration in _limitConfigurationList.LimitConfigurations.Where(x => x.Level != UserLevel.User))
            {
                var lists = await _traktRepository.Get(limitConfiguration.Level);

                foreach (var traktList in lists)
                {
                    _backgroundJobQueueService.Queue(traktList);
                }
            }
        }

    }
}