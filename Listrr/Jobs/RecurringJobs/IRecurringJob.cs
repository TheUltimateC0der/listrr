using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Listrr.BackgroundJob
{
    public interface IRecurringJob
    {
        Task Execute();
    }


    public interface IRecurringJob<T>
    {
        Task Execute(T param);
    }
}