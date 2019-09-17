using System;
using System.Threading.Tasks;
using Listrr.Data.Trakt;
using Listrr.Jobs.BackgroundJobs;
using Listrr.Services;
using Microsoft.AspNetCore.Identity;

namespace Listrr.BackgroundJob
{
    public class ProcessListsRecurringJob : IRecurringJob
    {

        private readonly ITraktService traktService;
        private readonly UserManager<IdentityUser> userManager;

        public ProcessListsRecurringJob(ITraktService traktService, UserManager<IdentityUser> userManager)
        {
            this.traktService = traktService;
            this.userManager = userManager;
        }


        public async Task Execute()
        {
            var lists = await traktService.GetProcessable();

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