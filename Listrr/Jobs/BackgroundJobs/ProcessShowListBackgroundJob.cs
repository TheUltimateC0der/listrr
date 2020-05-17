using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Hangfire;

using Listrr.Comparer;
using Listrr.Configuration;
using Listrr.Data.Trakt;
using Listrr.Extensions;
using Listrr.Jobs.RecurringJobs;
using Listrr.Repositories;
using Listrr.Services;

using TraktNet.Exceptions;
using TraktNet.Objects.Get.Shows;

namespace Listrr.Jobs.BackgroundJobs
{
    public class ProcessShowListBackgroundJob : IBackgroundJob<uint>
    {
        private readonly ITraktService _traktService;
        private readonly ITraktListRepository _traktRepository;
        private readonly TraktAPIConfiguration _traktApiConfiguration;

        private TraktList traktList;

        public ProcessShowListBackgroundJob(ITraktService traktService, TraktAPIConfiguration traktApiConfiguration, ITraktListRepository traktRepository)
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

                if (string.IsNullOrWhiteSpace(traktList.ItemList))
                {
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
                }
                else
                {
                    var add = new List<ITraktShow>();
                    var regex = new Regex(@"(.*)\(([0-9]{4})\)");

                    foreach (var line in traktList.ItemList.Split("\r\n"))
                    {
                        var processedLine = regex.Match(line);
                        if (processedLine.Success && processedLine.Groups.Count == 3)
                        {
                            var cleanShowName = processedLine.Groups[1].Value.Trim();
                            var itemYearParseResult = int.TryParse(processedLine.Groups[2].Value, out var showYear);
                            if (itemYearParseResult)
                            {
                                var itemResult = await _traktService.ShowSearch(traktList, cleanShowName, showYear);
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
                        foreach (var toAddChunk in add.ChunkBy(_traktApiConfiguration.ChunkBy))
                        {
                            await _traktService.AddShows(toAddChunk, traktList);
                        }
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

                    if (forceRefresh)
                        await _traktService.Update(traktList);

                    await _traktRepository.Update(traktList);
                }
            }

            if (queueNext)
                BackgroundJob.Schedule<ProcessUserListsRecurringJob>(x => x.Execute(), TimeSpan.FromSeconds(_traktApiConfiguration.DelayRequeue));
        }

        [Queue("donor")]
        public async Task ExecutePriorized(uint param, bool queueNext = false, bool forceRefresh = false)
        {
            await Execute(param, queueNext);
        }
    }
}