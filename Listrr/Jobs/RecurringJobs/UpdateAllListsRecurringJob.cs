using Hangfire;

using Listrr.Services;

using System.Threading.Tasks;

namespace Listrr.Jobs.RecurringJobs
{

    [Queue("system")]
    public class UpdateAllListsRecurringJob : IRecurringJob
    {
        private readonly ITraktService _traktService;
        private readonly IBackgroundJobQueueService _backgroundJobQueueService;

        public UpdateAllListsRecurringJob(ITraktService traktService, IBackgroundJobQueueService backgroundJobQueueService)
        {
            _traktService = traktService;
            _backgroundJobQueueService = backgroundJobQueueService;
        }


        public async Task Execute()
        {
            var lists = await _traktService.GetLists();

            foreach (var traktList in lists)
            {
                _backgroundJobQueueService.Queue(traktList, false, true);
            }
        }

    }
}