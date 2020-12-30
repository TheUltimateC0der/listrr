using Listrr.Data;
using Listrr.Data.IMDb;

using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;

namespace Listrr.Repositories
{
    public class IMDbRepository : IIMDbRepository
    {
        private readonly AppDbContext _appDbContext;

        public IMDbRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IMDbRating> Get(string imdbId)
        {
            return await _appDbContext.ImDbRatings.FirstOrDefaultAsync(x => x.IMDbId == imdbId);
        }

        public async Task<IMDbRating> Create(IMDbRating model)
        {
            var result = await _appDbContext.ImDbRatings.AddAsync(model);

            await _appDbContext.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<IMDbRating> Update(IMDbRating model)
        {
            var result = _appDbContext.ImDbRatings.Update(model);

            await _appDbContext.SaveChangesAsync();

            return result.Entity;
        }
    }
}