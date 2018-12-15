using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Listrr.Data.Trakt;
using Listrr.Repositories;
using Microsoft.AspNetCore.Identity;

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


        public async Task<TraktList> Create(TraktList model)
        {
            model = await traktListApiRepository.Create(model);
            model = await traktListDbRepository.Create(model);

            return model;
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

        public async Task<TraktList> Update(TraktList model)
        {
            model = await traktListApiRepository.Update(model);
            model = await traktListDbRepository.Update(model);

            return model;
        }
    }
}