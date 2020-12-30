using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.GZip;

using Listrr.Data.IMDb;
using Listrr.Repositories;

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Listrr.Jobs.RecurringJobs.IMDb
{
    public class IMDbRatingsRecurringJob : IRecurringJob
    {
        private readonly IIMDbRepository _imDbRepository;

        public IMDbRatingsRecurringJob(IIMDbRepository imDbRepository)
        {
            _imDbRepository = imDbRepository;
        }

        public async Task Execute()
        {
            using var webClient = new WebClient();

            await webClient.DownloadFileTaskAsync("https://datasets.imdbws.com/title.ratings.tsv.gz", "title.ratings.tsv.gz");

            ExtractGZip("title.ratings.tsv.gz", Directory.GetCurrentDirectory());

            var votingLines = await File.ReadAllLinesAsync("title.ratings.tsv");

            foreach (var votingLine in votingLines)
            {
                if (votingLine.StartsWith("tt"))
                {
                    var votingParts = votingLine.Split("\t");
                    if (votingParts.Length == 3)
                    {
                        var imdbId = votingParts[0];
                        var imdbRating = Convert.ToSingle(votingParts[1]);
                        var imdbVotes = Convert.ToInt32(votingParts[2]);

                        var existingVoting = await _imDbRepository.Get(imdbId);
                        if (existingVoting != null)
                        {
                            if (Math.Abs(existingVoting.Rating - imdbRating) > 0.0 ||
                                existingVoting.Votes != imdbVotes)
                            {
                                existingVoting.Rating = imdbRating;
                                existingVoting.Votes = imdbVotes;

                                await _imDbRepository.Update(existingVoting);
                            }
                        }
                        else
                        {
                            await _imDbRepository.Create(new IMDbRating
                            {
                                IMDbId = imdbId,
                                Votes = imdbVotes,
                                Rating = imdbRating
                            });
                        }
                    }
                }
            }
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