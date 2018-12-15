using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Listrr.Data.Trakt;

namespace Listrr.Repositories
{
    public interface ITraktListAPIRepository
    {

        Task<TraktList> Get(uint id);
        //Task<TraktList> GetShared();

        //Task<IEnumerable<TraktList>> Get(IEnumerable<int> ids);
        //Task<IEnumerable<TraktList>> Get(IEnumerable<int> ids, int userId);

        Task<TraktList> Create(TraktList model);
        Task<TraktList> Update(TraktList model);
        
    }
}