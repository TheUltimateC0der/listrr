using System.Linq;
using System.Threading.Tasks;

using Hangfire;

using Listrr.Data;
using Listrr.Services;

using Microsoft.EntityFrameworkCore;

namespace Listrr.Jobs.RecurringJobs
{

    [Queue("system")]
    public class SetDonorsRecurringJob : IRecurringJob
    {
        private readonly IGitHubGraphService _gitHubGraphService;
        private readonly AppDbContext _appDbContext;

        public SetDonorsRecurringJob(IGitHubGraphService gitHubGraphService, AppDbContext appDbContext)
        {
            _gitHubGraphService = gitHubGraphService;
            _appDbContext = appDbContext;
        }

        public async Task Execute()
        {
            var newDonors = await _gitHubGraphService.GetDonor();
            var currentDonors = await _appDbContext.Users.Where(x => x.Level != UserLevel.User).ToListAsync();

            //Deactivate non donors
            foreach (var donor in currentDonors)
            {
                var login = await _appDbContext.UserLogins.FirstOrDefaultAsync(x => x.LoginProvider == "GitHub" && x.UserId == donor.Id);
                if (login != null)
                {
                    if (!newDonors.Contains(login.ProviderKey))
                    {
                        donor.Level = UserLevel.User;

                        await _appDbContext.SaveChangesAsync();
                    }
                }
            }
            
            //Activate donors
            foreach (var donor in newDonors)
            {
                var login = await _appDbContext.UserLogins.FirstOrDefaultAsync(x => x.LoginProvider == "GitHub" && x.ProviderKey == donor);
                if (login != null)
                {
                    var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == login.UserId && x.Level == UserLevel.User);
                    if (user != null)
                    {
                        user.Level = UserLevel.Donor;

                        await _appDbContext.SaveChangesAsync();
                    }
                }
            }
        }

    }
}