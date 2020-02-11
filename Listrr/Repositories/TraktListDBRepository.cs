using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Listrr.Data;
using Listrr.Data.Trakt;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Listrr.Repositories
{
    public class TraktListDBRepository : ITraktListDBRepository
    {
        private readonly AppDbContext appDbContext;

        public TraktListDBRepository(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }


        public async Task<TraktList> Create(TraktList model)
        {
            appDbContext.TraktLists.Add(model);
            await appDbContext.SaveChangesAsync();

            return model;
        }

        public async Task<IList<TraktList>> Top(int count, int threshold)
        {
            return await appDbContext.TraktLists
                .Include(x => x.Owner)
                .OrderByDescending(x => x.Likes)
                .Take(count)
                .Where(x => x.Likes > threshold)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TraktList> Get(uint id)
        {
            return await appDbContext.TraktLists
                .Include(x => x.Owner)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IList<TraktList>> Get(IdentityUser user)
        {
            return await appDbContext.TraktLists
                .Include(x => x.Owner)
                .Where(x => x.Owner.Id == user.Id)
                .OrderBy(x => x.LastProcessed)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IList<TraktList>> Get(UserLevel userLevel)
        {
            return await appDbContext.TraktLists
                .Include(x => x.Owner)
                .Where(x => x.Process && x.Owner.Level == userLevel)
                .OrderBy(x => x.LastProcessed)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IList<TraktList>> Get(UserLevel userLevel, int take)
        {
            return await appDbContext.TraktLists
                .Include(x => x.Owner)
                .Where(x => x.Process && x.Owner.Level == userLevel)
                .OrderBy(x => x.LastProcessed)
                .Take(take)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TraktList> Update(TraktList model)
        {
            appDbContext.TraktLists.Update(model);
            await appDbContext.SaveChangesAsync();

            return model;
        }

        public async Task Delete(TraktList model)
        {
            appDbContext.TraktLists.Remove(model);

            await appDbContext.SaveChangesAsync();
        }
    }
}