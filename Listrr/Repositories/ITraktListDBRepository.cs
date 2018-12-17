using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Listrr.Data.Trakt;
using Microsoft.AspNetCore.Identity;

namespace Listrr.Repositories
{
    public interface ITraktListDBRepository
    {

        Task<TraktList> Get(uint id);
        Task<List<TraktList>> Get(IdentityUser user);

        Task<List<TraktList>> GetProcessable();

        Task<TraktList> Create(TraktList model);
        Task<TraktList> Update(TraktList model);
        
    }
}