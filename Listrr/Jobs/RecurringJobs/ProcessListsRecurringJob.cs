using System;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.States;
using Listrr.Data;
using Listrr.Data.Trakt;
using Listrr.Jobs.BackgroundJobs;
using Listrr.Services;

namespace Listrr.Jobs.RecurringJobs
{

    [Queue("System")]
    public class ProcessListsRecurringJob : IRecurringJob
    {

        private readonly ITraktService _traktService;

        public ProcessListsRecurringJob(ITraktService traktService)
        {
            _traktService = traktService;
        }


        public async Task Execute()
        {
            var normalLists = await _traktService.GetLists(User.UserLevel.User);
            var donorLists = await _traktService.GetLists(User.UserLevel.Donor);

            IBackgroundJobClient donorQueueClient = new BackgroundJobClient();
            var donorEnqueuedState = new EnqueuedState { Queue = "Donor" };

            foreach (var traktList in donorLists)
            {
                switch (traktList.Type)
                {
                    case ListType.Movie:
                        donorQueueClient.Create<ProcessMovieListBackgroundJob>(x => x.Execute(traktList.Id), donorEnqueuedState);

                        break;
                    case ListType.Show:
                        donorQueueClient.Create<ProcessShowListBackgroundJob>(x => x.Execute(traktList.Id), donorEnqueuedState);

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            foreach (var traktList in normalLists)
            {
                switch (traktList.Type)
                {
                    case ListType.Movie:
                        Hangfire.BackgroundJob.Enqueue<ProcessMovieListBackgroundJob>(x => x.Execute(traktList.Id));

                        break;
                    case ListType.Show:
                        Hangfire.BackgroundJob.Enqueue<ProcessShowListBackgroundJob>(x => x.Execute(traktList.Id));

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        }

    }
}