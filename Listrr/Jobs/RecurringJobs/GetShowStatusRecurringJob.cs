using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Listrr.Data;
using Listrr.Data.Trakt;
using Microsoft.EntityFrameworkCore;

namespace Listrr.BackgroundJob
{
    public class GetShowStatusRecurringJob : IRecurringJob
    {

        private readonly AppDbContext appDbContext;

        public GetShowStatusRecurringJob(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }


        public async Task Execute()
        {
            var result = new List<string> {"returning series", "in production", "planned", "canceled", "ended"};

            var currentStatus = await appDbContext.TraktShowStatuses.ToListAsync();

            foreach (var traktStatus in result)
            {
                if (currentStatus.All(x => x.Name != traktStatus))
                {
                    await appDbContext.TraktShowStatuses.AddAsync(new TraktShowStatus()
                    {
                        Name = traktStatus
                    });
                    await appDbContext.SaveChangesAsync();
                }
            }
        }
    }
}