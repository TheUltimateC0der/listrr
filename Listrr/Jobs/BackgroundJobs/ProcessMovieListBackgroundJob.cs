using Hangfire;
using Hangfire.Console;
using Hangfire.Server;

using Listrr.Comparer;
using Listrr.Configuration;
using Listrr.Data.Trakt;
using Listrr.Exceptions;
using Listrr.Extensions;
using Listrr.Jobs.RecurringJobs;
using Listrr.Repositories;
using Listrr.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using TraktNet.Exceptions;
using TraktNet.Objects.Get.Movies;

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

        public async Task Execute(uint param, PerformContext context, bool queueNext = false, bool forceRefresh = false)
        {
            var addProgressBar = context.WriteProgressBar();
            var removeProgressBar = context.WriteProgressBar();

            try
            {
                traktList = await _traktRepository.Get(param);
                traktList = await _traktService.Get(traktList);

                traktList.ScanState = ScanState.Updating;

                await _traktRepository.Update(traktList);

                if (string.IsNullOrWhiteSpace(traktList.ItemList))
                {
                    var found = await _traktService.MovieSearch(traktList);
                    var existing = await _traktService.GetMovies(traktList);

                    var remove = existing.Except(found, new TraktMovieComparer()).ToList();
                    var add = found.Except(existing, new TraktMovieComparer()).ToList();

                    if (add.Any())
                    {
                        foreach (var toAddChunk in add.ChunkBy(_traktApiConfiguration.ChunkBy).WithProgress(addProgressBar))
                        {
                            await _traktService.AddMovies(toAddChunk, traktList);
                        }
                    }

                    if (remove.Any())
                    {
                        foreach (var toRemoveChunk in remove.ChunkBy(_traktApiConfiguration.ChunkBy).WithProgress(removeProgressBar))
                        {
                            await _traktService.RemoveMovies(toRemoveChunk, traktList);
                        }
                    }
                }
                else
                {
                    var add = new List<ITraktMovie>();
                    var regex = new Regex(@"(.*)\(([0-9]{4})\)");

                    foreach (var line in traktList.ItemList.Split("\r\n"))
                    {
                        var processedLine = regex.Match(line);
                        if (processedLine.Success && processedLine.Groups.Count == 3)
                        {
                            var cleanMovieName = processedLine.Groups[1].Value.Trim();
                            var itemYearParseResult = int.TryParse(processedLine.Groups[2].Value, out var movieYear);
                            if (itemYearParseResult)
                            {
                                var itemResult = await _traktService.MovieSearch(traktList, cleanMovieName, movieYear);
                                if (itemResult != null)
                                {
                                    add.Add(itemResult);
                                }

                                await Task.Delay(_traktApiConfiguration.DelayIdSearch);
                            }
                        }
                    }

                    if (add.Any())
                    {
                        foreach (var toAddChunk in add.ChunkBy(_traktApiConfiguration.ChunkBy).WithProgress(addProgressBar))
                        {
                            await _traktService.AddMovies(toAddChunk, traktList);
                        }
                    }
                }
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
                else if (ex is TraktAuthenticationOAuthException || ex is TraktAuthorizationException || ex is RefreshTokenBadRequestException)
                {
                    traktList = await _traktRepository.Get(param);
                    traktList.Process = false;
                }
                else if (ex is ArgumentOutOfRangeException)
                {

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

                    if (forceRefresh)
                        await _traktService.Update(traktList);

                    traktList.LastProcessed = DateTime.Now;

                    await _traktRepository.Update(traktList);
                }
            }

            if (queueNext)
                BackgroundJob.Schedule<ProcessUserListsRecurringJob>(x => x.Execute(), TimeSpan.FromSeconds(_traktApiConfiguration.DelayRequeue));
        }

        [Queue("donor")]
        public async Task ExecutePriorized(uint param, PerformContext context, bool queueNext = false, bool forceRefresh = false)
        {
            await Execute(param, context, queueNext, forceRefresh);
        }
    }
}