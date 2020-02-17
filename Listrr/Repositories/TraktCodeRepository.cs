using Listrr.Data;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Listrr.Repositories
{
    public class TraktCodeRepository : ITraktCodeRepository
    {
        private readonly AppDbContext _appDbContext;

        public TraktCodeRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<CountryCode> CreateCountryCode(CountryCode model)
        {
            _appDbContext.CountryCodes.Add(model);
            await _appDbContext.SaveChangesAsync();

            return model;
        }

        public async Task<CountryCode> GetCountryCode(string name)
        {
            return await _appDbContext.CountryCodes.FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<IList<CountryCode>> GetCountryCodes()
        {
            return await _appDbContext.CountryCodes.OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<LanguageCode> GetLanguageCode(string name)
        {
            return await _appDbContext.LanguageCodes.FirstOrDefaultAsync(x => x.Name == name);
        }

        public async Task<IList<LanguageCode>> GetLanguageCodes()
        {
            return await _appDbContext.LanguageCodes.OrderBy(x => x.Name).ToListAsync();
        }
    }
}