using System;
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


            // Just return couse we get the id from trakt
            return model;
        }

        public async Task<TraktList> Get(uint id)
        {
            return await appDbContext.TraktLists.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<TraktList>> Get(IdentityUser user)
        {
            //if (await appDbContext.TraktLists.AnyAsync(x => x.Owner.Id == user.Id))
            //{
                return await appDbContext.TraktLists.Where(x => x.Owner.Id == user.Id)?.ToListAsync();
            //}

            return null;
        }

        public async Task<TraktList> Update(TraktList model)
        {
            appDbContext.TraktLists.Update(model);
            await appDbContext.SaveChangesAsync();


            // Just return couse we do sanatizing before
            return model;
        }
    }
}