using System;

using Hangfire;

using Listrr.Data;
using Listrr.Data.Trakt;
using Listrr.Jobs.BackgroundJobs;

namespace Listrr.Services
{
    public class BackgroundJobQueueService : IBackgroundJobQueueService
    {
        public void Queue(TraktList list, bool queueNext = false, bool forceRefresh = false)
        {
            switch (list.Type)
            {
                case ListType.Movie:
                    MovieList(list, queueNext, forceRefresh);

                    break;
                case ListType.Show:
                    ShowList(list, queueNext, forceRefresh);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private void MovieList(TraktList traktList, bool queueNext = false, bool forceRefresh = false)
        {
            switch (traktList.Owner.Level)
            {
                case UserLevel.User:
                    BackgroundJob.Enqueue<ProcessMovieListBackgroundJob>(x => x.Execute(traktList.Id, queueNext, forceRefresh));

                    break;
                case UserLevel.Donor:
                case UserLevel.DonorPlus:
                case UserLevel.DonorPlusPlus:
                case UserLevel.Developer:
                    BackgroundJob.Enqueue<ProcessMovieListBackgroundJob>(x => x.ExecutePriorized(traktList.Id, queueNext, forceRefresh));

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(traktList.Owner.Level), traktList.Owner.Level, null);
            }
        }
        
        private void ShowList(TraktList traktList, bool queueNext = false, bool forceRefresh = false)
        {
            switch (traktList.Owner.Level)
            {
                case UserLevel.User:
                    BackgroundJob.Enqueue<ProcessShowListBackgroundJob>(x => x.Execute(traktList.Id, queueNext, forceRefresh));

                    break;
                case UserLevel.Donor:
                case UserLevel.DonorPlus:
                case UserLevel.DonorPlusPlus:
                case UserLevel.Developer:
                    BackgroundJob.Enqueue<ProcessShowListBackgroundJob>(x => x.ExecutePriorized(traktList.Id, queueNext, forceRefresh));

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(traktList.Owner.Level), traktList.Owner.Level, null);
            }
        }
    }
}