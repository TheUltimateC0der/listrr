using Hangfire;
using Hangfire.Server;

using Listrr.Data.Trakt;
using Listrr.Repositories;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Listrr.Jobs.RecurringJobs
{

    [Queue("system")]
    public class GetShowStatusRecurringJob : IRecurringJob
    {
        private readonly ITraktShowRepository _traktShowRepository;

        public GetShowStatusRecurringJob(ITraktShowRepository traktShowRepository)
        {
            _traktShowRepository = traktShowRepository;
        }


        public async Task Execute(PerformContext context)
        {
            var result = new List<string>
            {
                TraktNet.Enums.TraktShowStatus.Ended.ObjectName,
                TraktNet.Enums.TraktShowStatus.InProduction.ObjectName,
                TraktNet.Enums.TraktShowStatus.ReturningSeries.ObjectName,
                TraktNet.Enums.TraktShowStatus.Canceled.ObjectName
            };

            var currentStatuses = await _traktShowRepository.GetStatuses();

            foreach (var traktStatus in result)
            {
                var currentStatus = currentStatuses.FirstOrDefault(x => x.Name == traktStatus);

                if (currentStatus == null)
                {
                    await _traktShowRepository.CreateStatus(new TraktShowStatus()
                    {
                        Name = traktStatus
                    });
                }
            }
        }
    }
}