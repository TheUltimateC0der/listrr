using Listrr.Data;
using Listrr.Data.Trakt;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Listrr.Repositories
{
    public class TraktShowRepository : ITraktShowRepository
    {
        private readonly AppDbContext _appDbContext;

        public TraktShowRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<TraktShowCertification> CreateCertification(TraktShowCertification model)
        {
            await _appDbContext.TraktShowCertifications.AddAsync(model);
            await _appDbContext.SaveChangesAsync();

            return model;
        }

        public async Task<TraktShowGenre> CreateGenre(TraktShowGenre model)
        {
            await _appDbContext.TraktShowGenres.AddAsync(model);
            await _appDbContext.SaveChangesAsync();

            return model;
        }

        public async Task<TraktShowNetwork> CreateNetwork(TraktShowNetwork model)
        {
            await _appDbContext.TraktShowNetworks.AddAsync(model);
            await _appDbContext.SaveChangesAsync();

            return model;
        }

        public async Task<TraktShowStatus> CreateStatus(TraktShowStatus model)
        {
            await _appDbContext.TraktShowStatuses.AddAsync(model);
            await _appDbContext.SaveChangesAsync();

            return model;
        }

        public async Task<IList<TraktShowCertification>> GetCertifications()
        {
            return await _appDbContext.TraktShowCertifications.OrderBy(x => x.Description).ToListAsync();
        }
        
        public async Task<IList<TraktShowGenre>> GetGenres()
        {
            return await _appDbContext.TraktShowGenres.ToListAsync();
        }

        public async Task<IList<TraktShowNetwork>> GetNetworks()
        {
            return await _appDbContext.TraktShowNetworks.OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<IList<TraktShowStatus>> GetStatuses()
        {
            return await _appDbContext.TraktShowStatuses.OrderBy(x => x.Name).ToListAsync();
        }
    }
}
