using System.Collections.Generic;
using System.Threading.Tasks;

using Listrr.Data.Trakt;
using Microsoft.AspNetCore.Identity;
using TraktNet.Objects.Get.Movies;
using TraktNet.Objects.Get.Shows;

namespace Listrr.Repositories
{
    public interface ITraktListAPIRepository
    {

        Task<TraktList> Get(uint id, IdentityUser user = null);
        Task<IList<ITraktMovie>> GetMovies(TraktList model);
        Task<IList<ITraktShow>> GetShows(TraktList model);
        
        Task<TraktList> Create(TraktList model);
        Task<TraktList> Update(TraktList model);
        Task Delete(TraktList model);

        Task<bool> Exists(TraktList model);



        Task AddMovies(IList<ITraktMovie> movies, TraktList list);

        Task RemoveMovies(IEnumerable<ITraktMovie> movies, TraktList list);


        Task AddShows(IList<ITraktShow> shows, TraktList list);

        Task RemoveShows(IEnumerable<ITraktShow> shows, TraktList list);

        Task<IList<ITraktMovie>> MovieSearch(TraktList model);
        Task<IList<ITraktShow>> ShowSearch(TraktList model);

    }
}