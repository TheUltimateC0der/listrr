using Listrr.Data;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Listrr.Repositories
{
    public interface ITraktCodeRepository
    {

        Task<CountryCode> CreateCountryCode(CountryCode model);
        Task<CountryCode> GetCountryCode(string name);
        Task<IList<CountryCode>> GetCountryCodes();

        Task<LanguageCode> CreateLanguageCode(LanguageCode model);
        Task<LanguageCode> GetLanguageCode(string name);
        Task<IList<LanguageCode>> GetLanguageCodes();

    }
}