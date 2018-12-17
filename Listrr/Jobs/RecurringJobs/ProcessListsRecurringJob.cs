using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Listrr.Data;
using Listrr.Data.Trakt;
using Listrr.Jobs.BackgroundJobs;
using Listrr.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TraktNet;
using TraktNet.Objects.Authentication;

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
                Hangfire.BackgroundJob.Enqueue<ProcessListBackgroundJob>(x => x.Execute(traktList.Id));
            }

        }

    }
}