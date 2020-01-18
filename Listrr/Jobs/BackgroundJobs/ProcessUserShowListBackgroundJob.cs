using System;
using System.Linq;
using System.Threading.Tasks;

using Listrr.Comparer;
using Listrr.Data.Trakt;
using Listrr.Extensions;
using Listrr.Services;

using TraktNet.Exceptions;

namespace Listrr.Jobs.BackgroundJobs
{
    public class ProcessUserShowListBackgroundJob : IBackgroundJob<uint>
    {
        private readonly ITraktService _traktService;
        private TraktList traktList;

        public ProcessUserShowListBackgroundJob(ITraktService traktService)
        {
            _traktService = traktService;
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
                    foreach (var toAddChunk in add.ChunkBy(500))
                    {
                        await _traktService.AddShows(toAddChunk, traktList);
                    }
                }

                if (remove.Any())
                {
                    foreach (var toRemoveChunk in remove.ChunkBy(500))
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
    }
}