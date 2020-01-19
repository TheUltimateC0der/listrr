using System.Linq;
using System.Threading.Tasks;

using Hangfire;

using Listrr.Configuration;
using Listrr.Data;
using Listrr.Services;

using Microsoft.EntityFrameworkCore;

namespace Listrr.Jobs.RecurringJobs
{
    [Queue("system")]
    public class EnforceListLimitRecurringJob : IRecurringJob
    {
        private readonly AppDbContext _appDbContext;
        private readonly ITraktService _traktService;
        private readonly LimitConfigurationList _limitConfigurationList;

        public EnforceListLimitRecurringJob(AppDbContext appDbContext, LimitConfigurationList limitConfigurationList, ITraktService traktService)
        {
            _appDbContext = appDbContext;
            _limitConfigurationList = limitConfigurationList;
            _traktService = traktService;
        }


        public async Task Execute()
        {
            var users = await _appDbContext.Users.ToListAsync();

            foreach (var user in users)
            {
                var limitConfig = _limitConfigurationList.LimitConfigurations.First(x => x.Level == user.Level);
                var lists = await _traktService.Get(user);
                
                if (lists.Count > limitConfig.ListLimit)
                {
                    foreach (var traktList in lists)
                    {
                        traktList.Process = false;

                        await _traktService.Update(traktList);
                    }
                }
                else
                {
                    foreach (var traktList in lists)
                    {
                        traktList.Process = true;

                        await _traktService.Update(traktList);
                    }
                }
            }

        }
    }
}