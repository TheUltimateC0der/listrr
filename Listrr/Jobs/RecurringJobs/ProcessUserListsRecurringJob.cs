using System.Threading.Tasks;

using Hangfire;

using Listrr.Data;
using Listrr.Services;

namespace Listrr.Jobs.RecurringJobs
{

    [Queue("system")]
    public class ProcessUserListsRecurringJob : IRecurringJob
    {
        private readonly ITraktService _traktService;
        private readonly IBackgroundJobQueueService _backgroundJobQueueService;

        public ProcessUserListsRecurringJob(ITraktService traktService, IBackgroundJobQueueService backgroundJobQueueService)
        {
            _traktService = traktService;
            _backgroundJobQueueService = backgroundJobQueueService;
        }


        public async Task Execute()
        {
            var lists = await _traktService.GetLists(UserLevel.User, 1);

            foreach (var traktList in lists)
            {
                _backgroundJobQueueService.Queue(traktList, true);
            }
        }

    }
}