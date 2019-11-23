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
            var countries = new List<RegionInfo>();
            foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                var country = new RegionInfo(culture.LCID);
                if (countries.All(p => p.Name != country.Name))
                    countries.Add(country);
            }

            foreach (var regionInfo in countries)
            {
                if (appDbContext.CountryCodes.Any(x => x.Code == regionInfo.TwoLetterISORegionName.ToLower())) continue;
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