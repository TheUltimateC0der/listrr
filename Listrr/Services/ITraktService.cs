using System.Collections.Generic;
using System.Threading.Tasks;
using Listrr.Data.Trakt;
using Microsoft.AspNetCore.Identity;
using TraktNet.Objects.Get.Movies;
using TraktNet.Objects.Get.Shows;

namespace Listrr.Services
{
    public interface ITraktService
    {

        Task<TraktList> Get(uint id, bool forceAPI = false);
        Task<List<TraktList>> Get(IdentityUser user);

        Task<List<ITraktMovie>> GetMovies(TraktList model);
        Task<List<ITraktShow>> GetShows(TraktList model);

        Task<List<TraktList>> GetProcessable();


        Task AddMovies(IEnumerable<ITraktMovie> movies, TraktList list);
        Task RemoveMovies(IEnumerable<ITraktMovie> movies, TraktList list);


        Task AddShows(IEnumerable<ITraktShow> movies, TraktList list);
        Task RemoveShows(IEnumerable<ITraktShow> movies, TraktList list);


        Task<TraktList> Create(TraktList model);
        Task<TraktList> Update(TraktList model);


        Task<List<ITraktMovie>> MovieSearch(TraktList model);
        Task<List<ITraktShow>> ShowSearch(TraktList model);

    }
}