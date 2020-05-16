using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Hangfire;

using Listrr.Data;
using Listrr.Repositories;

namespace Listrr.Jobs.RecurringJobs
{

    [Queue("system")]
    public class GetCountryCodesRecurringJob : IRecurringJob
    {
        private readonly ITraktCodeRepository _traktCodeRepository;

        public GetCountryCodesRecurringJob(ITraktCodeRepository traktCodeRepository)
        {
            _traktCodeRepository = traktCodeRepository;
        }


        public async Task Execute()
        {
            var countries = new List<RegionInfo>();
            foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                try
                {
                    var country = new RegionInfo(culture.LCID);
                    if (countries.All(p => p.Name != country.Name))
                        countries.Add(country);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            foreach (var regionInfo in countries)
            {
                var countryCode = await _traktCodeRepository.GetCountryCode(regionInfo.TwoLetterISORegionName.ToLower());

                if (countryCode != null) continue;
                if (regionInfo.TwoLetterISORegionName.Length != 2) continue;

                await _traktCodeRepository.CreateCountryCode(
                    new CountryCode()
                    {
                        Code = regionInfo.TwoLetterISORegionName.ToLower(),
                        Name = regionInfo.DisplayName
                    }
                );
            }


        }
    }
}