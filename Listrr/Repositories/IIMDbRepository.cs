using Listrr.Data.IMDb;

using System.Threading.Tasks;

namespace Listrr.Repositories
{
    public interface IIMDbRepository
    {
        Task<IMDbRating> Get(string imdbId);

        Task<IMDbRating> Create(IMDbRating model);

        Task<IMDbRating> Update(IMDbRating model);
    }
}