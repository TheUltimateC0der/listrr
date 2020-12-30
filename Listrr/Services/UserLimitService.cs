using Listrr.Configuration;
using Listrr.Data;

using System.Linq;

namespace Listrr.Services
{
    public class UserLimitService : IUserLimitService
    {
        private readonly LimitConfigurationList _limitConfigurationList;

        public UserLimitService(LimitConfigurationList limitConfigurationList)
        {
            _limitConfigurationList = limitConfigurationList;
        }


        public LimitConfiguration Get(UserLevel userLevel)
        {
            return _limitConfigurationList.LimitConfigurations.FirstOrDefault(x => x.Level == userLevel);
        }
    }
}
