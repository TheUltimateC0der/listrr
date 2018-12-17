using System.Linq;
using System.Threading.Tasks;
using Listrr.Comparer;
using Listrr.Services;

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

            var moviesToRemove = existingMovies.Except(foundMovies, new TraktMovieComparer()).ToList();
            var moviesToAdd = foundMovies.Except(existingMovies, new TraktMovieComparer()).ToList();

            if(moviesToAdd.Any())
                await traktService.AddMovies(moviesToAdd, list);

            if(moviesToRemove.Any())
                await traktService.RemoveMovies(moviesToRemove, list);
        }
    }
}