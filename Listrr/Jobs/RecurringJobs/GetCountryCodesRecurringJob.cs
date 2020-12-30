using Hangfire;
using Hangfire.Server;

using Listrr.Data;
using Listrr.Repositories;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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


        public async Task Execute(PerformContext context)
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

            var dbCountryCodes = await _traktCodeRepository.GetCountryCodes();
            foreach (var regionInfo in countries)
            {
                var countryCode = dbCountryCodes.FirstOrDefault(x => x.Code == regionInfo.TwoLetterISORegionName.ToLower(CultureInfo.InvariantCulture));

                if (countryCode != null) continue;
                if (regionInfo.TwoLetterISORegionName.Length != 2) continue;

                await _traktCodeRepository.CreateCountryCode(
                    new CountryCode()
                    {
                        Code = regionInfo.TwoLetterISORegionName.ToLower(CultureInfo.InvariantCulture),
                        Name = regionInfo.DisplayName
                    }
                );
            }
        }
    }
}