using System.Threading.Tasks;

using Hangfire.Server;

namespace Listrr.Jobs.BackgroundJobs
{
    public interface IBackgroundJob
    {
        Task Execute(PerformContext context);
    }


    public interface IBackgroundJob<T>
    {
        Task Execute(T param, PerformContext context, bool queueNext = false, bool forceRefresh = false);

        Task ExecutePriorized(T param, PerformContext context, bool queueNext = false, bool forceRefresh = false);
    }
}