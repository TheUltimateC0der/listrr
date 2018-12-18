using System.Collections.Generic;
using System.Threading.Tasks;
using Listrr.Data.Trakt;
using TraktNet.Objects.Get.Movies;
using TraktNet.Objects.Get.Shows;

namespace Listrr.Repositories
{
    public interface ITraktListAPIRepository
    {

        Task<TraktList> Get(uint id);
        Task<List<ITraktMovie>> GetMovies(TraktList model);
        Task<List<ITraktShow>> GetShows(TraktList model);
        
        Task<TraktList> Create(TraktList model);
        Task<TraktList> Update(TraktList model);

        Task AddMovies(IEnumerable<ITraktMovie> movies, TraktList list);

        Task RemoveMovies(IEnumerable<ITraktMovie> movies, TraktList list);


        Task AddShows(IEnumerable<ITraktShow> shows, TraktList list);

        Task RemoveShows(IEnumerable<ITraktShow> shows, TraktList list);

        Task<List<ITraktMovie>> MovieSearch(TraktList model);
        Task<List<ITraktShow>> ShowSearch(TraktList model);

    }
}