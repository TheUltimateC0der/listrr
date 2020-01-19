using System.Linq;
using System.Threading.Tasks;

using Hangfire;

using Listrr.Configuration;
using Listrr.Data;
using Listrr.Services;

namespace Listrr.Jobs.RecurringJobs
{

    [Queue("system")]
    public class ProcessDonorListsRecurringJob : IRecurringJob
    {
        private readonly ITraktService _traktService;
        private readonly LimitConfigurationList _limitConfigurationList;
        private readonly IBackgroundJobQueueService _backgroundJobQueueService;

        public ProcessDonorListsRecurringJob(ITraktService traktService, LimitConfigurationList limitConfigurationList, IBackgroundJobQueueService backgroundJobQueueService)
        {
            _traktService = traktService;
            _limitConfigurationList = limitConfigurationList;
            _backgroundJobQueueService = backgroundJobQueueService;
        }


        public async Task Execute()
        {
            foreach (var limitConfiguration in _limitConfigurationList.LimitConfigurations.Where(x => x.Level != UserLevel.User))
            {
                var lists = await _traktService.GetLists(limitConfiguration.Level);

                foreach (var traktList in lists)
                {
                    _backgroundJobQueueService.Queue(traktList);
                }
            }
        }

    }
}