using Hangfire;

using Listrr.Services;

using System.Threading.Tasks;
using Listrr.Repositories;

namespace Listrr.Jobs.RecurringJobs
{

    [Queue("system")]
    public class UpdateAllListsRecurringJob : IRecurringJob
    {
        private readonly ITraktListRepository _traktRepository;
        private readonly IBackgroundJobQueueService _backgroundJobQueueService;

        public UpdateAllListsRecurringJob(IBackgroundJobQueueService backgroundJobQueueService, ITraktListRepository traktRepository)
        {
            _backgroundJobQueueService = backgroundJobQueueService;
            _traktRepository = traktRepository;
        }


        public async Task Execute()
        {
            var lists = await _traktRepository.Get();

            foreach (var traktList in lists)
            {
                _backgroundJobQueueService.Queue(traktList, false, true);
            }
        }

    }
}