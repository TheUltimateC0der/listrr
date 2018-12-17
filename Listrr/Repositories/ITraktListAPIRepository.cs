using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Listrr.Data.Trakt;
using Listrr.Models;
using TraktNet.Objects.Get.Movies;
using TraktNet.Objects.Post.Scrobbles;

namespace Listrr.Repositories
{
    public interface ITraktListAPIRepository
    {

        Task<TraktList> Get(uint id);
        Task<List<ITraktMovie>> GetMovies(TraktList model);
        
        Task<TraktList> Create(TraktList model);
        Task<TraktList> Update(TraktList model);

        Task AddMovies(IEnumerable<ITraktMovie> movies, TraktList list);

        Task RemoveMovies(IEnumerable<ITraktMovie> movies, TraktList list);

        Task<List<ITraktMovie>> MovieSearch(TraktList model);

    }
}