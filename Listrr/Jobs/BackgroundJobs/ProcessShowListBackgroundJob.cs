using System;
using System.Linq;
using System.Threading.Tasks;

using Hangfire;

using Listrr.Comparer;
using Listrr.Configuration;
using Listrr.Data.Trakt;
using Listrr.Extensions;
using Listrr.Services;

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

        public async Task Execute(uint param)
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
                await _traktService.Delete(new TraktList { Id = param });
            }
            finally
            {
                traktList.ScanState = ScanState.None;

                await _traktService.Update(traktList);
            }
        }

        [Queue("donor")]
        public async Task ExecutePriorized(uint param)
        {
            await Execute(param);
        }
    }
}