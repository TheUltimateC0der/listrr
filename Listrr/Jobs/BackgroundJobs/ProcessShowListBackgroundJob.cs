using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Listrr.Comparer;
using Listrr.Data.Trakt;
using Listrr.Extensions;
using Listrr.Services;

using TraktNet.Exceptions;
using TraktNet.Objects.Get.Shows;

namespace Listrr.Jobs.BackgroundJobs
{
    public class ProcessShowListBackgroundJob : IBackgroundJob<uint>
    {
        private readonly ITraktService _traktService;
        private TraktList traktList;

        public ProcessShowListBackgroundJob(ITraktService traktService)
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

                var toRemove = new List<ITraktShow>();
                foreach (var existingShow in existing)
                {
                    if (!found.Contains(existingShow, new TraktShowComparer()))
                        toRemove.Add(existingShow);
                }

                var toAdd = new List<ITraktShow>();
                foreach (var foundShow in found)
                {
                    if (!existing.Contains(foundShow, new TraktShowComparer()))
                        toAdd.Add(foundShow);
                }

                if (toAdd.Any())
                {
                    //Chunking to 100 items per list cause trakt api does not like 10000s of items
                    foreach (var toAddChunk in toAdd.ChunkBy(100))
                    {
                        await _traktService.AddShows(toAddChunk, traktList);
                    }
                }

                if (toRemove.Any())
                {
                    //Chunking to 100 items per list cause trakt api does not like 10000s of items
                    foreach (var toRemoveChunk in toRemove.ChunkBy(100))
                    {
                        await _traktService.RemoveShows(toRemoveChunk, traktList);
                    }
                }
                
                traktList.LastProcessed = DateTime.Now;
            }
            catch (TraktListNotFoundException)
            {
                await _traktService.Delete(await _traktService.Get(param));
            }
            finally
            {
                traktList.ScanState = ScanState.None;

                await _traktService.Update(traktList);
            }
        }
    }
}