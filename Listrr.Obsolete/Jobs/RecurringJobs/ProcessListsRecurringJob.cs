using Listrr.Data.Trakt;
using Listrr.Jobs.BackgroundJobs;
using Listrr.Services;

using System;
using System.Threading.Tasks;

namespace Listrr.Jobs.RecurringJobs
{
    public class ProcessListsRecurringJob : IRecurringJob
    {

        private readonly ITraktService _traktService;

        public ProcessListsRecurringJob(ITraktService traktService)
        {
            _traktService = traktService;
        }


        public async Task Execute()
        {
            var lists = await _traktService.GetProcessable();

            foreach (var traktList in lists)
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