using System.Threading.Tasks;

using Hangfire;

using Listrr.Data;
using Listrr.Repositories;
using Listrr.Services;

namespace Listrr.Jobs.RecurringJobs
{

    [Queue("system")]
    public class ProcessUserListsRecurringJob : IRecurringJob
    {
        private readonly ITraktListRepository _traktRepository;
        private readonly IBackgroundJobQueueService _backgroundJobQueueService;

        public ProcessUserListsRecurringJob(IBackgroundJobQueueService backgroundJobQueueService, ITraktListRepository traktRepository)
        {
            _backgroundJobQueueService = backgroundJobQueueService;
            _traktRepository = traktRepository;
        }


        public async Task Execute()
        {
            var list = await _traktRepository.GetNextForUpdate(UserLevel.User);
            if (list != null)
            {
                _backgroundJobQueueService.Queue(list, true);
            }
        }

    }
}