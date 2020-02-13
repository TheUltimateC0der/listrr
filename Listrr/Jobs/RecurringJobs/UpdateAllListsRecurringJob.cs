using Hangfire;

using Listrr.Services;

using System.Threading.Tasks;

namespace Listrr.Jobs.RecurringJobs
{

    [Queue("system")]
    public class UpdateAllListsRecurringJob : IRecurringJob
    {
        private readonly ITraktService _traktService;

        public UpdateAllListsRecurringJob(ITraktService traktService)
        {
            _traktService = traktService;
        }


        public async Task Execute()
        {
            var lists = await _traktService.GetLists();

            foreach (var traktList in lists)
            {
                await _traktService.Update(traktList, true);

                await Task.Delay(500);
            }
        }

    }
}