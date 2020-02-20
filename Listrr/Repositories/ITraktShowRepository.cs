using Listrr.Data.Trakt;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Listrr.Repositories
{
    public interface ITraktShowRepository
    {
        Task<TraktShowCertification> CreateCertification(TraktShowCertification model);
        Task<IList<TraktShowCertification>> GetCertifications();

        Task<TraktShowGenre> CreateGenre(TraktShowGenre model);
        Task<IList<TraktShowGenre>> GetGenres();

        Task<TraktShowNetwork> CreateNetwork(TraktShowNetwork model);
        Task<IList<TraktShowNetwork>> GetNetworks();

        Task<TraktShowStatus> CreateStatus(TraktShowStatus model);
        Task<IList<TraktShowStatus>> GetStatuses();
    }
}