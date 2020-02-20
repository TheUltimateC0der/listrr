using Listrr.Data.Trakt;

using System.Collections.Generic;
using System.Threading.Tasks;

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