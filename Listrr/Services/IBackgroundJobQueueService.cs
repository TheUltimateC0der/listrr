using Listrr.Data.Trakt;

namespace Listrr.Services
{
    public interface IBackgroundJobQueueService
    {

        void Queue(TraktList list, bool queueNext = false, bool forceRefresh = false);

    }
}