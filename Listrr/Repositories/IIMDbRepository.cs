using Listrr.Data.IMDb;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Listrr.Repositories
{
    public interface IIMDbRepository
    {
        Task<IMDbRating> Get(string imdbId);

        Task<IMDbRating> Create(IMDbRating model);
        Task Create(IEnumerable<IMDbRating> models);

        Task<IMDbRating> Update(IMDbRating model);
    }
}