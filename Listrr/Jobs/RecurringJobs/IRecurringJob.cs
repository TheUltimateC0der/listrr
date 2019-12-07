using System.Threading.Tasks;

namespace Listrr.Jobs.RecurringJobs
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