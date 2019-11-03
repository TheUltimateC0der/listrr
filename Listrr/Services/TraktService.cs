using Listrr.Data.Trakt;
using Listrr.Repositories;

using Microsoft.AspNetCore.Identity;

using System.Collections.Generic;
using System.Threading.Tasks;

using TraktNet.Objects.Get.Movies;
using TraktNet.Objects.Get.Shows;

namespace Listrr.Services
{
    public class TraktService : ITraktService
    {

        private readonly ITraktListAPIRepository _traktListApiRepository;
        private readonly ITraktListDBRepository _traktListDbRepository;

        public TraktService(ITraktListDBRepository traktListDbRepository, ITraktListAPIRepository traktListApiRepository)
        {
            this._traktListDbRepository = traktListDbRepository;
            this._traktListApiRepository = traktListApiRepository;
        }

        public async Task AddMovies(IList<ITraktMovie> movies, TraktList list)
        {
            await _traktListApiRepository.AddMovies(movies, list);
        }

        public async Task AddShows(IList<ITraktShow> shows, TraktList list)
        {
            await _traktListApiRepository.AddShows(shows, list);
        }

        public async Task<TraktList> Create(TraktList model)
        {
            model = await _traktListApiRepository.Create(model);
            model = await _traktListDbRepository.Create(model);

            return model;
        }

        public async Task Delete(TraktList model, bool onlyLocal = true)
        {
            await _traktListDbRepository.Delete(model);

            if(!onlyLocal)
                await _traktListApiRepository.Delete(model);
        }

        public async Task<TraktList> Get(uint id, bool forceAPI = false)
        {
            if (forceAPI)
            {
                var dbResult = await _traktListDbRepository.Get(id);
                var apiResult = await _traktListApiRepository.Get(id, dbResult.Owner);

                dbResult.Items = apiResult.Items;
                dbResult.Likes = apiResult.Likes;

                return dbResult;
            }

            //since we always update the db after updating via api, we only need get via DB
            return await _traktListDbRepository.Get(id);
        }

        public Task<List<TraktList>> Get(IdentityUser user)
        {
            return _traktListDbRepository.Get(user);
        }

        public Task<IList<TraktList>> Top(int count, int threshold)
        {
            return _traktListDbRepository.Top(count, threshold);
        }

        public async Task<IList<ITraktMovie>> GetMovies(TraktList model)
        {
            return await _traktListApiRepository.GetMovies(model);
        }

        public async Task<bool> Exists(TraktList model)
        {
            return await _traktListApiRepository.Exists(model);
        }

        public async Task<IList<TraktList>> GetProcessable()
        {
            return await _traktListDbRepository.GetProcessable();
        }

        public async Task<IList<ITraktShow>> GetShows(TraktList model)
        {
            return await _traktListApiRepository.GetShows(model);
        }

        public async Task<IList<ITraktMovie>> MovieSearch(TraktList model)
        {
            return await _traktListApiRepository.MovieSearch(model);
        }

        public async Task RemoveMovies(IEnumerable<ITraktMovie> movies, TraktList list)
        {
            await _traktListApiRepository.RemoveMovies(movies, list);
        }

        public async Task RemoveShows(IEnumerable<ITraktShow> shows, TraktList list)
        {
            await _traktListApiRepository.RemoveShows(shows, list);
        }

        public async Task<IList<ITraktShow>> ShowSearch(TraktList model)
        {
            return await _traktListApiRepository.ShowSearch(model);
        }

        public async Task<TraktList> Update(TraktList model)
        {
            await _traktListApiRepository.Update(model);

            return await _traktListDbRepository.Update(model);
            
        }
    }
}