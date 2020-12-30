using Listrr.Configuration;
using Listrr.Data;

namespace Listrr.Services
{
    public interface IUserLimitService
    {
        LimitConfiguration Get(UserLevel userLevel);
    }
}