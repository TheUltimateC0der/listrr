using Listrr.Configuration;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Listrr.Services
{
    public interface IGitHubGraphService
    {

        Task<IDictionary<string, LimitConfiguration>> GetDonor();

    }
}