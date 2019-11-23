using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Listrr.Data;

namespace Listrr.BackgroundJob
{
    public class GetCountryCodesRecurringJob : IRecurringJob
    {

        private readonly AppDbContext appDbContext;

        public GetCountryCodesRecurringJob(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }


        public async Task Execute()
        {
            List<RegionInfo> countries = new List<RegionInfo>();
            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo country = new RegionInfo(culture.LCID);
                if (countries.All(p => p.Name != country.Name))
                    countries.Add(country);
            }

            foreach (var regionInfo in countries)
            {
                if (!appDbContext.CountryCodes.Any(x => x.Code == regionInfo.TwoLetterISORegionName.ToLower()))
                {
                    if(regionInfo.TwoLetterISORegionName.Length != 2) continue;

                    await appDbContext.CountryCodes.AddAsync(
                        new CountryCode()
                        {
                            Code = regionInfo.TwoLetterISORegionName.ToLower(),
                            Name = regionInfo.DisplayName
                        }
                    );

                    await appDbContext.SaveChangesAsync();
                }
            }


        }
    }
}