using Hangfire;

using Listrr.Comparer;
using Listrr.Configuration;
using Listrr.Data.Trakt;
using Listrr.Extensions;
using Listrr.Jobs.RecurringJobs;
using Listrr.Services;

using System;
using System.Linq;
using System.Threading.Tasks;

using TraktNet.Exceptions;

namespace Listrr.Jobs.BackgroundJobs
{
    public class ProcessShowListBackgroundJob : IBackgroundJob<uint>
    {
        private readonly ITraktService _traktService;
        private readonly TraktAPIConfiguration _traktApiConfiguration;
        private TraktList traktList;

        public ProcessShowListBackgroundJob(ITraktService traktService, TraktAPIConfiguration traktApiConfiguration)
        {
            _traktService = traktService;
            _traktApiConfiguration = traktApiConfiguration;
        }

        public async Task Execute(uint param, bool queueNext = false)
        {
            try
            {
                traktList = await _traktService.Get(param, true);
                traktList.ScanState = ScanState.Updating;

                await _traktService.Update(traktList);

                var found = await _traktService.ShowSearch(traktList);
                var existing = await _traktService.GetShows(traktList);

                var remove = existing.Except(found, new TraktShowComparer()).ToList();
                var add = found.Except(existing, new TraktShowComparer()).ToList();

                if (add.Any())
                {
                    foreach (var toAddChunk in add.ChunkBy(_traktApiConfiguration.ChunkBy))
                    {
                        await _traktService.AddShows(toAddChunk, traktList);
                    }
                }

                if (remove.Any())
                {
                    foreach (var toRemoveChunk in remove.ChunkBy(_traktApiConfiguration.ChunkBy))
                    {
                        await _traktService.RemoveShows(toRemoveChunk, traktList);
                    }
                }

                traktList.LastProcessed = DateTime.Now;
            }
            catch (TraktListNotFoundException)
            {
                if (traktList != null)
                {
                    await _traktService.Delete(traktList);
                    traktList = null;
                }
                else
                {
                    await _traktService.Delete(new TraktList {Id = param});
                }
            }
            catch (TraktAuthorizationException)
            {
                traktList = await _traktService.Get(param);
                traktList.LastProcessed = DateTime.Now;
                traktList.Process = false;
            }
            finally
            {
                if (traktList != null)
                {
                    traktList.ScanState = ScanState.None;

                    await _traktService.Update(traktList);
                }
            }

            if (queueNext)
                BackgroundJob.Enqueue<ProcessUserListsRecurringJob>(x => x.Execute());
        }

        [Queue("donor")]
        public async Task ExecutePriorized(uint param, bool queueNext = false)
        {
            await Execute(param, queueNext);
        }
    }
}