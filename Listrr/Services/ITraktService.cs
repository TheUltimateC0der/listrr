using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Listrr.Data.Trakt;
using Microsoft.AspNetCore.Identity;

namespace Listrr.Services
{
    public interface ITraktService
    {

        Task<TraktList> Get(uint id, bool forceAPI = false);
        Task<List<TraktList>> Get(IdentityUser user);
        //Task<TraktList> GetShared();

        //Task<IEnumerable<TraktList>> Get(IEnumerable<int> ids);
        //Task<IEnumerable<TraktList>> Get(IEnumerable<int> ids, int userId);

        Task<TraktList> Create(TraktList model);
        Task<TraktList> Update(TraktList model);

    }
}