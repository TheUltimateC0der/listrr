using System.Collections.Generic;
using System.Threading.Tasks;

using Listrr.Data;

namespace Listrr.Repositories
{
    public interface IUserRepository
    {

        Task<IList<User>> Get();

    }
}