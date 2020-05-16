using System.Collections.Generic;
using System.Threading.Tasks;

using Listrr.Data;

using Microsoft.EntityFrameworkCore;

namespace Listrr.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;

        public UserRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IList<User>> Get()
        {
            return await _appDbContext.Users.ToListAsync();
        }
    }
}