using System.Collections.Generic;
using System.Threading.Tasks;
using Listrr.Data.Trakt;
using Listrr.Repositories;
using Microsoft.AspNetCore.Identity;
using TraktNet.Objects.Get.Movies;
using TraktNet.Objects.Get.Shows;

namespace Listrr.Services
{
    public class TraktService : ITraktService
    {

        private readonly ITraktListAPIRepository traktListApiRepository;
        private readonly ITraktListDBRepository traktListDbRepository;

        public TraktService(ITraktListDBRepository traktListDbRepository, ITraktListAPIRepository traktListApiRepository)
        {
            this.traktListDbRepository = traktListDbRepository;
            this.traktListApiRepository = traktListApiRepository;
        }

        public async Task AddMovies(IList<ITraktMovie> movies, TraktList list)
        {
            await traktListApiRepository.AddMovies(movies, list);
        }

        public async Task AddShows(IList<ITraktShow> shows, TraktList list)
        {
            await traktListApiRepository.AddShows(shows, list);
        }

        public async Task<TraktList> Create(TraktList model)
        {
            model = await traktListApiRepository.Create(model);
            model = await traktListDbRepository.Create(model);

            return model;
        }

        public async Task Delete(TraktList model)
        {
            await traktListApiRepository.Delete(model);
        }

        public async Task<TraktList> Get(uint id, bool forceAPI = false)
        {
            if (forceAPI)
            {
                var apiResult = await traktListApiRepository.Get(id);
                var dbResult = await traktListDbRepository.Get(id);

                apiResult.LastProcessed = dbResult.LastProcessed;
                apiResult.Process = dbResult.Process;
                apiResult.Owner = dbResult.Owner;

                return apiResult;
            }

            //since we always update the db after updating via api, we only need get via DB
            return await traktListDbRepository.Get(id);
        }

        public Task<List<TraktList>> Get(IdentityUser user)
        {
            return traktListDbRepository.Get(user);
        }

        public async Task<IList<ITraktMovie>> GetMovies(TraktList model)
        {
            return await traktListApiRepository.GetMovies(model);
        }

        public async Task<IList<TraktList>> GetProcessable()
        {
            return await traktListDbRepository.GetProcessable();
        }

        public async Task<IList<ITraktShow>> GetShows(TraktList model)
        {
            return await traktListApiRepository.GetShows(model);
        }

        public async Task<IList<ITraktMovie>> MovieSearch(TraktList model)
        {
            return await traktListApiRepository.MovieSearch(model);
        }

        public async Task RemoveMovies(IEnumerable<ITraktMovie> movies, TraktList list)
        {
            await traktListApiRepository.RemoveMovies(movies, list);
        }

        public async Task RemoveShows(IEnumerable<ITraktShow> shows, TraktList list)
        {
            await traktListApiRepository.RemoveShows(shows, list);
        }

        public async Task<IList<ITraktShow>> ShowSearch(TraktList model)
        {
            return await traktListApiRepository.ShowSearch(model);
        }

        public async Task<TraktList> Update(TraktList model)
        {
            await traktListApiRepository.Update(model);

            return await traktListDbRepository.Update(model);
            
        }
    }
}