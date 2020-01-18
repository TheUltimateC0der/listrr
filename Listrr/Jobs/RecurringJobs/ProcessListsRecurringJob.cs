using System;
using System.Threading.Tasks;

using Hangfire;

using Listrr.Data;
using Listrr.Data.Trakt;
using Listrr.Jobs.BackgroundJobs;
using Listrr.Services;

namespace Listrr.Jobs.RecurringJobs
{

    [Queue("system")]
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
            
            foreach (var traktList in donorLists)
            {
                switch (traktList.Type)
                {
                    case ListType.Movie:
                        BackgroundJob.Enqueue<ProcessDonorMovieListBackgroundJob>(x => x.Execute(traktList.Id));

                        break;
                    case ListType.Show:
                        BackgroundJob.Enqueue<ProcessDonorShowListBackgroundJob>(x => x.Execute(traktList.Id));

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
                        BackgroundJob.Enqueue<ProcessUserMovieListBackgroundJob>(x => x.Execute(traktList.Id));

                        break;
                    case ListType.Show:
                        BackgroundJob.Enqueue<ProcessUserShowListBackgroundJob>(x => x.Execute(traktList.Id));

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        }

    }
}