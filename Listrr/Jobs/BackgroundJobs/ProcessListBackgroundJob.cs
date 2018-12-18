using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Listrr.Comparer;
using Listrr.Services;
using TraktNet.Objects.Get.Movies;

namespace Listrr.Jobs.BackgroundJobs
{
    public class ProcessListBackgroundJob : IBackgroundJob<uint>
    {

        private readonly ITraktService traktService;

        public ProcessListBackgroundJob(ITraktService traktService)
        {
            this.traktService = traktService;
        }

        public async Task Execute(uint param)
        {
            var list = await traktService.Get(param);
            var foundMovies = await traktService.MovieSearch(list);
            var existingMovies = await traktService.GetMovies(list);

            List<ITraktMovie> moviesToRemove = new List<ITraktMovie>();
            foreach (var existingMovie in existingMovies)
            {
                if(!foundMovies.Contains(existingMovie, new TraktMovieComparer()))
                    moviesToRemove.Add(existingMovie);
            }

            List<ITraktMovie> moviesToAdd = new List<ITraktMovie>();
            foreach (var foundMovie in foundMovies)
            {
                if (!existingMovies.Contains(foundMovie, new TraktMovieComparer()))
                    moviesToAdd.Add(foundMovie);
            }

            if (moviesToAdd.Any())
                await traktService.AddMovies(moviesToAdd, list);

            if(moviesToRemove.Any())
                await traktService.RemoveMovies(moviesToRemove, list);

            list.Items = (existingMovies.Count + moviesToAdd.Count - moviesToRemove.Count);
            list.LastProcessed = DateTime.Now;

            await traktService.Update(list);
        }
    }
}