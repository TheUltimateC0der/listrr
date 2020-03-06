using Listrr.Data;
using Listrr.Data.Trakt;

using Microsoft.AspNetCore.Identity;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Listrr.Repositories
{
    public interface ITraktListRepository
    {
        Task<IList<TraktList>> Get();
        Task<IList<TraktList>> Get(int take, int skip);

        Task<TraktList> Get(uint id);
        Task<IList<TraktList>> Get(IdentityUser user);
        Task<IList<TraktList>> Get(UserLevel userClass);
        Task<IList<TraktList>> Get(UserLevel userClass, int take);

        Task<int> Count();

        Task<TraktList> Create(TraktList model);
        Task<TraktList> Update(TraktList model);

        Task Delete(TraktList model);
    }
}