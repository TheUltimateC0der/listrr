using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Listrr.Comparer;
using Listrr.Data.Trakt;
using Listrr.Extensions;
using Listrr.Services;

using TraktNet.Exceptions;
using TraktNet.Objects.Get.Movies;

namespace Listrr.Jobs.BackgroundJobs
{
    public class ProcessMovieListBackgroundJob : IBackgroundJob<uint>
    {
        private readonly ITraktService _traktService;
        private TraktList traktList;
        
        public ProcessMovieListBackgroundJob(ITraktService traktService)
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

                var foundMovies = await _traktService.MovieSearch(traktList);
                var existingMovies = await _traktService.GetMovies(traktList);

                var moviesToRemove = new List<ITraktMovie>();
                foreach (var existingMovie in existingMovies)
                {
                    if (!foundMovies.Contains(existingMovie, new TraktMovieComparer()))
                        moviesToRemove.Add(existingMovie);
                }

                var moviesToAdd = new List<ITraktMovie>();
                foreach (var foundMovie in foundMovies)
                {
                    if (!existingMovies.Contains(foundMovie, new TraktMovieComparer()))
                        moviesToAdd.Add(foundMovie);
                }

                if (moviesToAdd.Any())
                {
                    //Chunking to 100 items per list cause trakt api does not like 10000s of items
                    foreach (var moviesToAddChunk in moviesToAdd.ChunkBy(500))
                    {
                        await _traktService.AddMovies(moviesToAddChunk, traktList);
                    }
                }

                if (moviesToRemove.Any())
                {
                    //Chunking to 100 items per list cause trakt api does not like 10000s of items
                    foreach (var moviesToRemoveChunk in moviesToRemove.ChunkBy(500))
                    {
                        await _traktService.RemoveMovies(moviesToRemoveChunk, traktList);
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