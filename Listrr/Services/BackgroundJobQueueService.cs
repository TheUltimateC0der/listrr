using System;

using Hangfire;

using Listrr.Data;
using Listrr.Data.Trakt;
using Listrr.Jobs.BackgroundJobs;

namespace Listrr.Services
{
    public class BackgroundJobQueueService : IBackgroundJobQueueService
    {
        public void Queue(TraktList list)
        {
            switch (list.Type)
            {
                case ListType.Movie:
                    MovieList(list);

                    break;
                case ListType.Show:
                    ShowList(list);

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }



        private void MovieList(TraktList traktList)
        {
            switch (traktList.Owner.Level)
            {
                case UserLevel.User:
                    BackgroundJob.Enqueue<ProcessMovieListBackgroundJob>(x => x.Execute(traktList.Id));

                    break;
                case UserLevel.Donor:
                    BackgroundJob.Enqueue<ProcessMovieListBackgroundJob>(x => x.ExecutePriorized(traktList.Id));

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(traktList.Owner.Level), traktList.Owner.Level, null);
            }
        }
        
        private void ShowList(TraktList traktList)
        {
            switch (traktList.Owner.Level)
            {
                case UserLevel.User:
                    BackgroundJob.Enqueue<ProcessShowListBackgroundJob>(x => x.Execute(traktList.Id));

                    break;
                case UserLevel.Donor:
                    BackgroundJob.Enqueue<ProcessShowListBackgroundJob>(x => x.ExecutePriorized(traktList.Id));

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(traktList.Owner.Level), traktList.Owner.Level, null);
            }
        }
    }
}