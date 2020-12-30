using Hangfire.Server;

using System.Threading.Tasks;

namespace Listrr.Jobs.RecurringJobs
{
    public interface IRecurringJob
    {
        Task Execute(PerformContext context);
    }


    public interface IRecurringJob<T>
    {
        Task Execute(T param, PerformContext context);
    }
}