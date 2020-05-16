using System.Linq;
using System.Threading.Tasks;

using Hangfire;

using Listrr.Configuration;
using Listrr.Repositories;

namespace Listrr.Jobs.RecurringJobs
{
    [Queue("system")]
    public class EnforceListLimitRecurringJob : IRecurringJob
    {
        private readonly IUserRepository _userRepository;
        private readonly ITraktListRepository _traktRepository;
        
        private readonly LimitConfigurationList _limitConfigurationList;

        public EnforceListLimitRecurringJob(LimitConfigurationList limitConfigurationList, IUserRepository userRepository, ITraktListRepository traktRepository)
        {
            _limitConfigurationList = limitConfigurationList;
            _userRepository = userRepository;
            _traktRepository = traktRepository;
        }


        public async Task Execute()
        {
            var users = await _userRepository.Get();

            foreach (var user in users)
            {
                var limitConfig = _limitConfigurationList.LimitConfigurations.First(x => x.Level == user.Level);
                var lists = await _traktRepository.Get(user);
                
                if (lists.Count > limitConfig.ListLimit)
                {
                    foreach (var traktList in lists.Where(x => x.Process))
                    {
                        traktList.Process = false;

                        await _traktRepository.Update(traktList);
                    }
                }
                else
                {
                    foreach (var traktList in lists.Where(x => x.Process == false))
                    {
                        traktList.Process = true;

                        await _traktRepository.Update(traktList);
                    }
                }
            }

        }
    }
}