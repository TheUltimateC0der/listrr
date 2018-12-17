using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Listrr.Data;
using Listrr.Data.Trakt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TraktNet;

namespace Listrr.BackgroundJob
{
    public class GetLanguageCodesRecurringJob : IRecurringJob
    {

        private readonly AppDbContext appDbContext;

        public GetLanguageCodesRecurringJob(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }


        public async Task Execute()
        {
            foreach (CultureInfo culture in CultureInfo.GetCultures(CultureTypes.NeutralCultures))
            {
                if (!appDbContext.LanguageCodes.Any(x => x.Code == culture.TwoLetterISOLanguageName && x.Name == culture.NativeName))
                {
                    await appDbContext.LanguageCodes.AddAsync(
                        new LanguageCode()
                        {
                            Code = culture.TwoLetterISOLanguageName,
                            Name = culture.NativeName
                        }
                    );

                    await appDbContext.SaveChangesAsync();
                }
            }
        }
    }
}