using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Listrr.Comparer;
using Listrr.Extensions;
using Listrr.Services;
using TraktNet.Objects.Get.Shows;

namespace Listrr.Jobs.BackgroundJobs
{
    public class ProcessShowListBackgroundJob : IBackgroundJob<uint>
    {

        private readonly ITraktService traktService;

        public ProcessShowListBackgroundJob(ITraktService traktService)
        {
            this.traktService = traktService;
        }

        public async Task Execute(uint param)
        {
            var list = await traktService.Get(param);
            var found = await traktService.ShowSearch(list);
            var existing = await traktService.GetShows(list);

            List<ITraktShow> toRemove = new List<ITraktShow>();
            foreach (var existingShow in existing)
            {
                if(!found.Contains(existingShow, new TraktShowComparer()))
                    toRemove.Add(existingShow);
            }

            List<ITraktShow> toAdd = new List<ITraktShow>();
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
                    await traktService.AddShows(toAddChunk, list);
                }
            }

            if (toRemove.Any())
            {
                //Chunking to 100 items per list cause trakt api does not like 10000s of items
                foreach (var toRemoveChunk in toRemove.ChunkBy(100))
                {
                    await traktService.RemoveShows(toRemoveChunk, list);
                }
            }

            list.Items = (existing.Count + toAdd.Count - toRemove.Count);
            list.LastProcessed = DateTime.Now;

            await traktService.Update(list);
        }
    }
}