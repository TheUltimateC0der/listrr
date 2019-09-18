using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Listrr.API.Trakt.Models.Filters;
using Listrr.Data;
using Listrr.Data.Trakt;
using Listrr.Jobs.BackgroundJobs;
using Listrr.Models;
using Listrr.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using TraktNet.Enums;

using TraktShowStatus = Listrr.Data.Trakt.TraktShowStatus;

namespace Listrr.Controllers
{
    public class HomeController : Controller
    {

        private readonly ITraktService _traktService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AppDbContext _appDbContext;

        public HomeController(ITraktService traktService, UserManager<IdentityUser> userManager, AppDbContext appDbContext)
        {
            _traktService = traktService;
            _userManager = userManager;
            _appDbContext = appDbContext;
        }
        

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Lists()
        {
            ViewData["Message"] = "Overview of your lists";

            return View(await _traktService.Get(await _userManager.GetUserAsync(User)));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MovieList()
        {
            ViewData["Message"] = "Create a new list for movies";

            var dbGenres = await _appDbContext.TraktMovieGenres.ToListAsync();
            var dbCertifications = await _appDbContext.TraktMovieCertifications.ToListAsync();
            var dbCountryCodes = await _appDbContext.CountryCodes.OrderBy(x => x.Name).ToListAsync();
            var dbLanguageCodes = await _appDbContext.LanguageCodes.OrderBy(x => x.Name).ToListAsync();
            
            var model = new CreateMovieListViewModel()
            {
                Genres = new MultiSelectList(dbGenres, nameof(TraktMovieGenre.Slug), nameof(TraktMovieGenre.Slug)),
                Certifications = new MultiSelectList(dbCertifications, nameof(TraktMovieCertification.Slug), nameof(TraktMovieCertification.Description)),
                Countries = new MultiSelectList(dbCountryCodes, nameof(CountryCode.Code), nameof(CountryCode.Name)),
                Languages = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Name)),
                Translations = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Name))
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MovieList(CreateMovieListViewModel model)
        {
            ViewData["Message"] = "Create a new list for movies";

            var dbGenres = await _appDbContext.TraktMovieGenres.ToListAsync();
            var dbCertifications = await _appDbContext.TraktMovieCertifications.ToListAsync();
            var dbCountryCodes = await _appDbContext.CountryCodes.OrderBy(x => x.Name).ToListAsync();
            var dbLanguageCodes = await _appDbContext.LanguageCodes.OrderBy(x => x.Name).ToListAsync();

            model.Genres = new MultiSelectList(dbGenres, nameof(TraktMovieGenre.Slug), nameof(TraktMovieGenre.Slug));
            model.Certifications = new MultiSelectList(dbCertifications, nameof(TraktMovieCertification.Slug), nameof(TraktMovieCertification.Description));
            model.Countries = new MultiSelectList(dbCountryCodes, nameof(CountryCode.Code), nameof(CountryCode.Name));
            model.Languages = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Name));
            model.Translations = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Name));

            if (!ModelState.IsValid) return View(model);

            TraktSearchField searchFields = new TraktSearchField();

            if (model.SearchByAliases) searchFields = searchFields | TraktSearchField.Aliases;
            if (model.SearchByBiography) searchFields = searchFields | TraktSearchField.Biography;
            if (model.SearchByDescription) searchFields = searchFields | TraktSearchField.Description;
            if (model.SearchByName) searchFields = searchFields | TraktSearchField.Name;
            if (model.SearchByOverview) searchFields = searchFields | TraktSearchField.Overview;
            if (model.SearchByPeople) searchFields = searchFields | TraktSearchField.People;
            if (model.SearchByTagline) searchFields = searchFields | TraktSearchField.Tagline;
            if (model.SearchByTitle) searchFields = searchFields | TraktSearchField.Title;
            if (model.SearchByTranslations) searchFields = searchFields | TraktSearchField.Translations;

            
            var result = await _traktService.Create(new TraktList()
            {
                Name = model.Name,
                Query = model.Query ?? "",
                Type = ListType.Movie,
                Filter_SearchField = searchFields,
                Filter_Years = model.Filter_Years,
                Filter_Ratings = model.Filter_Ratings,
                Filter_Runtimes = model.Filter_Runtimes,
                Filter_Genres = new GenresCommonFilter(model.Filter_Genres),
                Filter_Languages = new LanguagesCommonFilter(model.Filter_Languages),
                Filter_Translations = new TranslationsBasicFilter(model.Filter_Translations),
                Filter_Certifications_Movie = new CertificationsMovieFilter(model.Filter_Certifications),
                Filter_Countries = new CountriesCommonFilter(model.Filter_Countries),
                Owner = await _userManager.GetUserAsync(User)
            });

            Hangfire.BackgroundJob.Enqueue<ProcessMovieListBackgroundJob>(x => x.Execute(result.Id));

            return RedirectToAction(nameof(MovieList));
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ShowList()
        {
            ViewData["Message"] = "Create a new list for shows";

            var dbGenres = await _appDbContext.TraktShowGenres.ToListAsync();
            var dbCertifications = await _appDbContext.TraktShowCertifications.OrderBy(x => x.Name).ToListAsync();
            var dbCountryCodes = await _appDbContext.CountryCodes.OrderBy(x => x.Name).ToListAsync();
            var dbLanguageCodes = await _appDbContext.LanguageCodes.OrderBy(x => x.Name).ToListAsync();
            var dbNetworks = await _appDbContext.TraktShowNetworks.OrderBy(x => x.Name).ToListAsync();
            var dbStatus = await _appDbContext.TraktShowStatuses.OrderBy(x => x.Name).ToListAsync();

            var model = new CreateShowListViewModel()
            {
                Genres = new MultiSelectList(dbGenres, nameof(TraktShowGenre.Slug), nameof(TraktShowGenre.Name)),
                Certifications = new MultiSelectList(dbCertifications, nameof(TraktShowCertification.Slug), nameof(TraktShowCertification.Description)),
                Countries = new MultiSelectList(dbCountryCodes, nameof(CountryCode.Code), nameof(CountryCode.Name)),
                Languages = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Name)),
                Translations = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Name)),
                Networks = new MultiSelectList(dbNetworks, nameof(TraktShowNetwork.Name), nameof(TraktShowNetwork.Name)),
                Status = new MultiSelectList(dbStatus, nameof(TraktShowStatus.Name), nameof(TraktShowStatus.Name)),
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ShowList(CreateShowListViewModel model)
        {
            ViewData["Message"] = "Create a new list for shows";

            var dbGenres = await _appDbContext.TraktShowGenres.ToListAsync();
            var dbCertifications = await _appDbContext.TraktShowCertifications.ToListAsync();
            var dbCountryCodes = await _appDbContext.CountryCodes.OrderBy(x => x.Name).ToListAsync();
            var dbLanguageCodes = await _appDbContext.LanguageCodes.OrderBy(x => x.Name).ToListAsync();
            var dbNetworks = await _appDbContext.TraktShowNetworks.ToListAsync();
            var dbStatus = await _appDbContext.TraktShowStatuses.ToListAsync();

            model.Genres = new MultiSelectList(dbGenres, nameof(TraktShowGenre.Slug), nameof(TraktShowGenre.Name));
            model.Certifications = new MultiSelectList(dbCertifications, nameof(TraktShowCertification.Slug), nameof(TraktShowCertification.Description));
            model.Countries = new MultiSelectList(dbCountryCodes, nameof(CountryCode.Code), nameof(CountryCode.Name));
            model.Languages = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Name));
            model.Translations = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Name));
            model.Networks = new MultiSelectList(dbNetworks, nameof(TraktShowNetwork.Name), nameof(TraktShowNetwork.Name));
            model.Status = new MultiSelectList(dbStatus, nameof(TraktShowStatus.Name), nameof(TraktShowStatus.Name));

            if (!ModelState.IsValid) return View(model);

            TraktSearchField searchFields = new TraktSearchField();

            if (model.SearchByAliases) searchFields = searchFields | TraktSearchField.Aliases;
            if (model.SearchByBiography) searchFields = searchFields | TraktSearchField.Biography;
            if (model.SearchByDescription) searchFields = searchFields | TraktSearchField.Description;
            if (model.SearchByName) searchFields = searchFields | TraktSearchField.Name;
            if (model.SearchByOverview) searchFields = searchFields | TraktSearchField.Overview;
            if (model.SearchByPeople) searchFields = searchFields | TraktSearchField.People;
            if (model.SearchByTitle) searchFields = searchFields | TraktSearchField.Title;
            if (model.SearchByTranslations) searchFields = searchFields | TraktSearchField.Translations;

            var result = await _traktService.Create(new TraktList()
            {
                Name = model.Name,
                Query = model.Query ?? "",
                Type = ListType.Show,
                Filter_SearchField = searchFields,
                Filter_Years = model.Filter_Years,
                Filter_Ratings = model.Filter_Ratings,
                Filter_Runtimes = model.Filter_Runtimes,
                Filter_Genres = new GenresCommonFilter(model.Filter_Genres),
                Filter_Languages = new LanguagesCommonFilter(model.Filter_Languages),
                Filter_Translations = new TranslationsBasicFilter(model.Filter_Translations),
                Filter_Certifications_Show = new CertificationsShowFilter(model.Filter_Certifications),
                Filter_Countries = new CountriesCommonFilter(model.Filter_Countries),
                Filter_Networks = new NetworksShowFilter(model.Filter_Networks),
                Filter_Status = new StatusShowFilter(model.Filter_Status),
                Owner = await _userManager.GetUserAsync(User)
            });

            Hangfire.BackgroundJob.Enqueue<ProcessShowListBackgroundJob>(x => x.Execute(result.Id));

            return RedirectToAction(nameof(ShowList));
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
