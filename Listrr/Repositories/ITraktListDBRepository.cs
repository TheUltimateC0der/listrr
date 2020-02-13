using System.Collections.Generic;
using System.Threading.Tasks;
using Listrr.Data;
using Listrr.Data.Trakt;
using Microsoft.AspNetCore.Identity;

namespace Listrr.Repositories
{
    public interface ITraktListDBRepository
    {

        Task<IList<TraktList>> Top(int count, int threshold);


        Task<IList<TraktList>> Get();

        Task<TraktList> Get(uint id);
        Task<IList<TraktList>> Get(IdentityUser user);

        Task<IList<TraktList>> Get(UserLevel userClass);
        Task<IList<TraktList>> Get(UserLevel userClass, int take);

        Task<TraktList> Create(TraktList model);
        Task<TraktList> Update(TraktList model);

        Task Delete(TraktList model);

    }
}