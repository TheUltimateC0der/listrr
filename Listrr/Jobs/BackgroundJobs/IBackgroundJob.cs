using System.Threading.Tasks;

namespace Listrr.Jobs.BackgroundJobs
{
    public interface IBackgroundJob
    {
        Task Execute();
    }


    public interface IBackgroundJob<T>
    {
        Task Execute(T param, bool queueNext = false, bool forceRefresh = false);

        Task ExecutePriorized(T param, bool queueNext = false, bool forceRefresh = false);
    }
}