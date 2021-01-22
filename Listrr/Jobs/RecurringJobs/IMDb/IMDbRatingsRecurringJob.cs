using Hangfire.Console;
using Hangfire.Server;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;

using Listrr.Data.IMDb;
using Listrr.Repositories;

using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Listrr.Jobs.RecurringJobs.IMDb
{
    public class IMDbRatingsRecurringJob : IRecurringJob
    {
        private readonly IIMDbRepository _imDbRepository;
        private readonly ILogger<IMDbRatingsRecurringJob> _logger;

        public IMDbRatingsRecurringJob(IIMDbRepository imDbRepository, ILogger<IMDbRatingsRecurringJob> logger)
        {
            _imDbRepository = imDbRepository;
            _logger = logger;
        }

        public async Task Execute(PerformContext context)
        {
            var pb = context.WriteProgressBar();

            using var webClient = new WebClient();

            await webClient.DownloadFileTaskAsync("https://datasets.imdbws.com/title.ratings.tsv.gz", "title.ratings.tsv.gz");

            ExtractGZip("title.ratings.tsv.gz", Directory.GetCurrentDirectory());

            var votingLines = await File.ReadAllLinesAsync("title.ratings.tsv");
            var items = new List<IMDbRating>();

            await _imDbRepository.Purge();

            foreach (var votingLine in votingLines.WithProgress(pb))
            {
                if (votingLine.StartsWith("tt"))
                {
                    var votingParts = votingLine.Split("\t");
                    if (votingParts.Length == 3)
                    {
                        var imdbId = votingParts[0];
                        var imdbRating = Convert.ToInt32(Convert.ToDouble(votingParts[1], CultureInfo.InvariantCulture) * 10);
                        var imdbVotes = Convert.ToInt32(votingParts[2]);


                        items.Add(new IMDbRating
                        {
                            IMDbId = imdbId,
                            Votes = imdbVotes,
                            Rating = imdbRating
                        });

                    }
                }
            }


            await _imDbRepository.CreateRange(items);
        }


        private void ExtractGZip(string gzipFileName, string targetDir)
        {
            // Use a 4K buffer. Any larger is a waste.    
            var dataBuffer = new byte[4096];

            using (Stream fs = new FileStream(gzipFileName, FileMode.Open, FileAccess.Read))
            {
                using (var gzipStream = new GZipInputStream(fs))
                {
                    // Change this to your needs
                    var fnOut = Path.Combine(targetDir, Path.GetFileNameWithoutExtension(gzipFileName));

                    using (var fsOut = File.Create(fnOut))
                    {
                        StreamUtils.Copy(gzipStream, fsOut, dataBuffer);
                    }
                }
            }
        }
    }
}