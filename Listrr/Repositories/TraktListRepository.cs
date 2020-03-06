using Listrr.Data;
using Listrr.Data.Trakt;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Listrr.Repositories
{
    public class TraktListRepository : ITraktListRepository
    {
        private readonly AppDbContext _appDbContext;

        public TraktListRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


        public async Task<TraktList> Create(TraktList model)
        {
            await _appDbContext.TraktLists.AddAsync(model);
            await _appDbContext.SaveChangesAsync();

            return model;
        }

        public async Task<IList<TraktList>> Get(int take, int skip)
        {
            return await _appDbContext.TraktLists
                .Include(x => x.Owner)
                .OrderByDescending(x => x.Likes)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IList<TraktList>> Get()
        {
            return await _appDbContext.TraktLists
                .Include(x => x.Owner)
                .ToListAsync();
        }

        public async Task<TraktList> Get(uint id)
        {
            return await _appDbContext.TraktLists
                .Include(x => x.Owner)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        
        public async Task<IList<TraktList>> Get(IdentityUser user)
        {
            return await _appDbContext.TraktLists
                .Include(x => x.Owner)
                .Where(x => x.Owner.Id == user.Id)
                .OrderBy(x => x.LastProcessed)
                .ToListAsync();
        }

        public async Task<IList<TraktList>> Get(UserLevel userLevel)
        {
            return await _appDbContext.TraktLists
                .Include(x => x.Owner)
                .Where(x => x.Process && x.Owner.Level == userLevel)
                .OrderBy(x => x.LastProcessed)
                .ToListAsync();
        }

        public async Task<IList<TraktList>> Get(UserLevel userLevel, int take)
        {
            return await _appDbContext.TraktLists
                .Include(x => x.Owner)
                .Where(x => x.Process && x.Owner.Level == userLevel)
                .OrderBy(x => x.LastProcessed)
                .Take(take)
                .ToListAsync();
        }


        public async Task<int> Count()
        {
            return await _appDbContext.TraktLists.CountAsync();
        }


        public async Task<TraktList> Update(TraktList model)
        {
            _appDbContext.TraktLists.Update(model);
            await _appDbContext.SaveChangesAsync();

            return model;
        }

        public async Task Delete(TraktList model)
        {
            _appDbContext.TraktLists.Remove(model);

            await _appDbContext.SaveChangesAsync();
        }
    }
}