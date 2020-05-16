using System.Collections.Generic;
using System.Threading.Tasks;

using Listrr.Configuration;

namespace Listrr.Services
{
    public interface IGitHubGraphService
    {

        Task<IDictionary<string, LimitConfiguration>> GetDonor();

    }
}