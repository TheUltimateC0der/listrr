using Listrr.Data;
using Listrr.Data.Trakt;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Listrr.Jobs.RecurringJobs
{
    public class GetShowStatusRecurringJob : IRecurringJob
    {

        private readonly AppDbContext _appDbContext;

        public GetShowStatusRecurringJob(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        public async Task Execute()
        {
            var result = new List<string>
            {
                TraktNet.Enums.TraktShowStatus.Ended.ObjectName,
                TraktNet.Enums.TraktShowStatus.InProduction.ObjectName,
                TraktNet.Enums.TraktShowStatus.ReturningSeries.ObjectName,
                TraktNet.Enums.TraktShowStatus.Canceled.ObjectName
            };

            var currentStatus = await _appDbContext.TraktShowStatuses.ToListAsync();

            foreach (var traktStatus in result)
            {
                if (currentStatus.All(x => x.Name != traktStatus))
                {
                    await _appDbContext.TraktShowStatuses.AddAsync(new TraktShowStatus()
                    {
                        Name = traktStatus
                    });
                    await _appDbContext.SaveChangesAsync();
                }
            }
        }
    }
}