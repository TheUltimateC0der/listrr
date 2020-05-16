using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Listrr.Data;
using Listrr.Data.Trakt;

using Microsoft.EntityFrameworkCore;

namespace Listrr.Repositories
{
    public class TraktMovieRepository : ITraktMovieRepository
    {
        private readonly AppDbContext _appDbContext;

        public TraktMovieRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<TraktMovieCertification> CreateCertification(TraktMovieCertification model)
        {
            await _appDbContext.TraktMovieCertifications.AddAsync(model);
            await _appDbContext.SaveChangesAsync();

            return model;
        }

        public async Task<TraktMovieGenre> CreateGenre(TraktMovieGenre model)
        {
            await _appDbContext.TraktMovieGenres.AddAsync(model);
            await _appDbContext.SaveChangesAsync();

            return model;
        }

        public async Task<IList<TraktMovieCertification>> GetCertifications()
        {
            return await _appDbContext.TraktMovieCertifications.OrderBy(x => x.Description).ToListAsync();
        }

        public async Task<IList<TraktMovieGenre>> GetGenres()
        {
            return await _appDbContext.TraktMovieGenres.ToListAsync();
        }
    }
}