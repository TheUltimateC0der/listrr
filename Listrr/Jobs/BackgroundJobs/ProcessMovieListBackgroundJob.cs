using Hangfire;

using Listrr.Comparer;
using Listrr.Configuration;
using Listrr.Data.Trakt;
using Listrr.Extensions;
using Listrr.Jobs.RecurringJobs;
using Listrr.Repositories;
using Listrr.Services;

using System;
using System.Linq;
using System.Threading.Tasks;

using TraktNet.Exceptions;

namespace Listrr.Jobs.BackgroundJobs
{
    public class ProcessMovieListBackgroundJob : IBackgroundJob<uint>
    {
        private readonly ITraktService _traktService;
        private readonly ITraktListRepository _traktRepository;
        private readonly TraktAPIConfiguration _traktApiConfiguration;

        private TraktList traktList;
        
        public ProcessMovieListBackgroundJob(ITraktService traktService, TraktAPIConfiguration traktApiConfiguration, ITraktListRepository traktRepository)
        {
            _traktService = traktService;
            _traktRepository = traktRepository;
            _traktApiConfiguration = traktApiConfiguration;
        }

        public async Task Execute(uint param, bool queueNext = false, bool forceRefresh = false)
        {
            try
            {
                traktList = await _traktRepository.Get(param);
                traktList = await _traktService.Get(traktList);

                traktList.ScanState = ScanState.Updating;
                
                await _traktRepository.Update(traktList);

                var found = await _traktService.MovieSearch(traktList);
                var existing = await _traktService.GetMovies(traktList);
                
                var remove = existing.Except(found, new TraktMovieComparer()).ToList();
                var add = found.Except(existing, new TraktMovieComparer()).ToList();

                if (add.Any())
                {
                    foreach (var toAddChunk in add.ChunkBy(_traktApiConfiguration.ChunkBy))
                    {
                        await _traktService.AddMovies(toAddChunk, traktList);
                    }
                }

                if (remove.Any())
                {
                    foreach (var toRemoveChunk in remove.ChunkBy(_traktApiConfiguration.ChunkBy))
                    {
                        await _traktService.RemoveMovies(toRemoveChunk, traktList);
                    }
                }

                traktList.LastProcessed = DateTime.Now;
            }
            catch (Exception ex)
            {
                if (ex is TraktListNotFoundException)
                {
                    if (traktList != null)
                    {
                        await _traktRepository.Delete(traktList);
                        traktList = null;
                    }
                    else
                    {
                        await _traktRepository.Delete(new TraktList { Id = param });
                    }
                }
                else if (ex is TraktAuthenticationOAuthException || ex is TraktAuthorizationException)
                {
                    traktList = await _traktRepository.Get(param);
                    traktList.LastProcessed = DateTime.Now;
                    traktList.Process = false;
                }
                else
                {
                    throw ex;
                }
            }
            finally
            {
                if (traktList != null)
                {
                    traktList.ScanState = ScanState.None;

                    if(forceRefresh)
                        await _traktService.Update(traktList);

                    await _traktRepository.Update(traktList);
                }
            }

            if(queueNext)
                BackgroundJob.Enqueue<ProcessUserListsRecurringJob>(x => x.Execute());
        }

        [Queue("donor")]
        public async Task ExecutePriorized(uint param, bool queueNext = false, bool forceRefresh = false)
        {
            await Execute(param, queueNext, forceRefresh);
        }
    }
}