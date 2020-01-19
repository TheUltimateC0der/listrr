using System.Collections.Generic;
using System.Threading.Tasks;

namespace Listrr.Services
{
    public interface IGitHubGraphService
    {

        Task<List<string>> GetDonor();

    }
}