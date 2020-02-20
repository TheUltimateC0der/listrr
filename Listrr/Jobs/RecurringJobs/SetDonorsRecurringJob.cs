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
    public class SetDonorsRecurringJob : IRecurringJob
    {
        private readonly UserMappingConfigurationList _userMappingConfigurationList;
        private readonly IGitHubGraphService _gitHubGraphService;
        private readonly AppDbContext _appDbContext;

        public SetDonorsRecurringJob(IGitHubGraphService gitHubGraphService, AppDbContext appDbContext, UserMappingConfigurationList userMappingConfigurationList)
        {
            _gitHubGraphService = gitHubGraphService;
            _appDbContext = appDbContext;
            _userMappingConfigurationList = userMappingConfigurationList;
        }

        public async Task Execute()
        {
            var newDonors = await _gitHubGraphService.GetDonor();
            var currentDonors = await _appDbContext.Users.Where(x => x.Level != UserLevel.User).ToListAsync();

            //Deactivate non donors
            foreach (var donor in currentDonors)
            {
                var login = await _appDbContext.UserLogins.FirstOrDefaultAsync(x => x.LoginProvider == "GitHub" && x.UserId == donor.Id);
                if (login == null) continue;
                if (newDonors.ContainsKey(login.ProviderKey)) continue;
                if (donor.Level == UserLevel.User) continue;

                donor.Level = UserLevel.User;

                await _appDbContext.SaveChangesAsync();
            }
            
            //Activate donors
            foreach (var newDonor in newDonors)
            {
                var login = await _appDbContext.UserLogins.FirstOrDefaultAsync(x => x.LoginProvider == "GitHub" && x.ProviderKey == newDonor.Key);
                if (login == null) continue;

                var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == login.UserId && x.Level != newDonor.Value.Level);
                if (user == null) continue;

                user.Level = newDonor.Value.Level;

                await _appDbContext.SaveChangesAsync();
            }

            //Hand edited donors
            foreach (var userMappingConfiguration in _userMappingConfigurationList.UserMappingConfigurations)
            {
                var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.UserName == userMappingConfiguration.User && x.Level != userMappingConfiguration.UserLevel);
                if (user == null) continue;
                if (user.Level == userMappingConfiguration.UserLevel) continue;

                user.Level = userMappingConfiguration.UserLevel;

                await _appDbContext.SaveChangesAsync();
            }
        }

    }
}