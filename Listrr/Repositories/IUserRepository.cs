using Listrr.Data;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Listrr.Repositories
{
    public interface IUserRepository
    {

        Task<IList<User>> Get();

    }
}