using System.Collections.Generic;
using System.Threading.Tasks;

using Listrr.Data.Trakt;

namespace Listrr.Repositories
{
    public interface ITraktMovieRepository
    {
        Task<TraktMovieCertification> CreateCertification(TraktMovieCertification model);
        Task<IList<TraktMovieCertification>> GetCertifications();

        Task<TraktMovieGenre> CreateGenre(TraktMovieGenre model);
        Task<IList<TraktMovieGenre>> GetGenres();
    }
}