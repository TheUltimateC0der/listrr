using Listrr.API.Trakt.Models.Filters;
using Listrr.Configuration;
using Listrr.Data;
using Listrr.Data.Trakt;
using Listrr.Models;
using Listrr.Repositories;
using Listrr.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using System.Linq;
using System.Threading.Tasks;

using TraktNet.Exceptions;

namespace Listrr.Controllers
{
    public class ListController : Controller
    {
        private readonly ITraktService _traktService;
        private readonly ITraktListRepository _traktRepository;
        private readonly ITraktMovieRepository _traktMovieRepository;
        private readonly ITraktShowRepository _traktShowRepository;
        private readonly ITraktCodeRepository _traktCodesRepository;
        private readonly UserManager<User> _userManager;
        private readonly IBackgroundJobQueueService _backgroundJobQueueService;
        private readonly LimitConfigurationList _limitConfigurationList;

        public ListController(UserManager<User> userManager, IBackgroundJobQueueService backgroundJobQueueService, LimitConfigurationList limitConfigurationList, ITraktListRepository traktRepository, ITraktMovieRepository traktMovieRepository, ITraktShowRepository traktShowRepository, ITraktCodeRepository traktCodesRepository, ITraktService traktService)
        {
            _userManager = userManager;
            _backgroundJobQueueService = backgroundJobQueueService;
            _limitConfigurationList = limitConfigurationList;
            _traktRepository = traktRepository;
            _traktMovieRepository = traktMovieRepository;
            _traktShowRepository = traktShowRepository;
            _traktCodesRepository = traktCodesRepository;
            _traktService = traktService;
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> My()
        {
            var user = await _userManager.GetUserAsync(User);
            var lists = await _traktRepository.Get(user);

            return View(new MyViewModel
            {
                TraktLists = lists,
                User = user
            });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Scan(uint id)
        {
            var user = await _userManager.GetUserAsync(User);
            var lists = await _traktRepository.Get(user);

            if (lists.Count > _limitConfigurationList.LimitConfigurations.First(x => x.Level == user.Level).ListLimit)
                return View("Error");


            var list = await _traktRepository.Get(id);
            if (list == null) return RedirectToAction(nameof(My));

            if (list.Owner.IsDonor && list.Owner.UserName == User.Identity.Name && list.ScanState == ScanState.None)
            {
                list.ScanState = ScanState.Scheduled;
                await _traktRepository.Update(list);

                _backgroundJobQueueService.Queue(list);
            }

            return RedirectToAction(nameof(My));
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MovieList()
        {
            var dbGenres = await _traktMovieRepository.GetGenres();
            var dbCertifications = await _traktMovieRepository.GetCertifications();
            var dbCountryCodes = await _traktCodesRepository.GetCountryCodes();
            var dbLanguageCodes = await _traktCodesRepository.GetLanguageCodes();

            var model = new CreateMovieListViewModel
            {
                Genres = new MultiSelectList(dbGenres, nameof(TraktMovieGenre.Slug), nameof(TraktMovieGenre.Slug)),
                Certifications = new MultiSelectList(dbCertifications, nameof(TraktMovieCertification.Slug), nameof(TraktMovieCertification.Description)),
                Countries = new MultiSelectList(dbCountryCodes, nameof(CountryCode.Code), nameof(CountryCode.Name)),
                Languages = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description)),
                Translations = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description)),
                ReverseGenres = new MultiSelectList(dbGenres, nameof(TraktMovieGenre.Slug), nameof(TraktMovieGenre.Name)),
                ReverseCertifications = new MultiSelectList(dbCertifications, nameof(TraktMovieCertification.Slug), nameof(TraktMovieCertification.Description)),
                ReverseCountries = new MultiSelectList(dbCountryCodes, nameof(CountryCode.Code), nameof(CountryCode.Name)),
                ReverseLanguages = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description)),
                ReverseTranslations = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description))
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MovieList(CreateMovieListViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var lists = await _traktRepository.Get(user);

            if (lists.Count >= _limitConfigurationList.LimitConfigurations.First(x => x.Level == user.Level).ListLimit)
                return View("Error");

            var dbGenres = await _traktMovieRepository.GetGenres();
            var dbCertifications = await _traktMovieRepository.GetCertifications();
            var dbCountryCodes = await _traktCodesRepository.GetCountryCodes();
            var dbLanguageCodes = await _traktCodesRepository.GetLanguageCodes();

            model.Genres = new MultiSelectList(dbGenres, nameof(TraktMovieGenre.Slug), nameof(TraktMovieGenre.Slug));
            model.Certifications = new MultiSelectList(dbCertifications, nameof(TraktMovieCertification.Slug), nameof(TraktMovieCertification.Description));
            model.Countries = new MultiSelectList(dbCountryCodes, nameof(CountryCode.Code), nameof(CountryCode.Name));
            model.Languages = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description));
            model.Translations = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description));
            model.ReverseGenres = new MultiSelectList(dbGenres, nameof(TraktMovieGenre.Slug), nameof(TraktMovieGenre.Name));
            model.ReverseCertifications = new MultiSelectList(dbCertifications, nameof(TraktMovieCertification.Slug), nameof(TraktMovieCertification.Description));
            model.ReverseCountries = new MultiSelectList(dbCountryCodes, nameof(CountryCode.Code), nameof(CountryCode.Name));
            model.ReverseLanguages = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description));
            model.ReverseTranslations = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description));

            if (!ModelState.IsValid) return View(model);

            TraktList result = null;

            try
            {
                result = await _traktService.Create(new TraktList
                {
                    Name = model.Name,
                    Query = model.Query ?? "",
                    Type = ListType.Movie,
                    Filter_Years = model.Filter_Years,
                    Filter_Ratings_Trakt = model.Filter_Ratings_Trakt,
                    Filter_Ratings_IMDb = model.Filter_Ratings_IMDb,
                    Filter_Runtimes = model.Filter_Runtimes,
                    SearchByAlias = model.SearchByAlias,
                    SearchByBiography = model.SearchByBiography,
                    SearchByDescription = model.SearchByDescription,
                    SearchByName = model.SearchByName,
                    SearchByOverview = model.SearchByOverview,
                    SearchByPeople = model.SearchByPeople,
                    SearchByTitle = model.SearchByTitle,
                    SearchByTranslations = model.SearchByTranslations,
                    SearchByTagline = model.SearchByTagline,
                    Filter_Genres = new GenresCommonFilter(model.Filter_Genres),
                    Filter_Languages = new LanguagesCommonFilter(model.Filter_Languages),
                    Filter_Translations = new TranslationsBasicFilter(model.Filter_Translations),
                    Filter_Certifications_Movie = new CertificationsMovieFilter(model.Filter_Certifications),
                    Filter_Countries = new CountriesCommonFilter(model.Filter_Countries),
                    ContentType = ListContentType.Filters,
                    Owner = user
                });
            }
            catch (TraktServerUnavailableException e)
            {
                return View(nameof(TraktServerUnavailableException));
            }

            if (user.IsDonor)
            {
                result.ExclusionFilter_Genres = new GenresCommonFilter(model.ExclusionFilter_Genres);
                result.ExclusionFilter_Languages = new LanguagesCommonFilter(model.ExclusionFilter_Languages);
                result.ExclusionFilter_Countries = new CountriesCommonFilter(model.ExclusionFilter_Countries);
                result.ExclusionFilter_Translations = new TranslationsBasicFilter(model.ExclusionFilter_Translations);
                result.ExclusionFilter_Certifications_Movie = new CertificationsMovieFilter(model.ExclusionFilter_Certifications);
                result.ExclusionFilter_Keywords = string.IsNullOrEmpty(model.ExclusionFilter_Keywords) ? null : model.ExclusionFilter_Keywords.Split(",");
            }

            await _traktRepository.Create(result);

            _backgroundJobQueueService.Queue(result);

            return RedirectToAction(nameof(MovieList));
        }

        [HttpGet]
        [Authorize]
        public IActionResult MovieListFile()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MovieListFile(CreateMovieListFileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var lists = await _traktRepository.Get(user);

            if (lists.Count >= _limitConfigurationList.LimitConfigurations.First(x => x.Level == user.Level).ListLimit)
                return View("Error");

            if (!_limitConfigurationList.LimitConfigurations.First(x => x.Level == user.Level).ListsFromNames)
                return View("Error");

            if (!ModelState.IsValid) return View(model);

            var result = await _traktService.Create(new TraktList
            {
                Name = model.Name,
                Type = ListType.Movie,
                ContentType = ListContentType.Names,
                ItemList = model.ItemList,
                Owner = user
            });

            await _traktRepository.Create(result);

            _backgroundJobQueueService.Queue(result);

            return RedirectToAction(nameof(MovieListFile));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditMovieList(uint id)
        {
            var list = await _traktRepository.Get(id);

            if (list == null) return RedirectToAction(nameof(My));
            if (list.Type != ListType.Movie) return RedirectToAction(nameof(My));

            if (list.Owner.UserName == User.Identity.Name)
            {
                var dbGenres = await _traktMovieRepository.GetGenres();
                var dbCertifications = await _traktMovieRepository.GetCertifications();
                var dbCountryCodes = await _traktCodesRepository.GetCountryCodes();
                var dbLanguageCodes = await _traktCodesRepository.GetLanguageCodes();

                return View(new EditMovieListViewModel
                {
                    Id = list.Id,
                    Genres = new MultiSelectList(dbGenres, nameof(TraktMovieGenre.Slug), nameof(TraktMovieGenre.Name)),
                    Certifications = new MultiSelectList(dbCertifications, nameof(TraktMovieCertification.Slug), nameof(TraktMovieCertification.Description)),
                    Countries = new MultiSelectList(dbCountryCodes, nameof(CountryCode.Code), nameof(CountryCode.Name)),
                    Languages = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description)),
                    Translations = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description)),
                    ReverseGenres = new MultiSelectList(dbGenres, nameof(TraktMovieGenre.Slug), nameof(TraktMovieGenre.Name)),
                    ReverseCertifications = new MultiSelectList(dbCertifications, nameof(TraktMovieCertification.Slug), nameof(TraktMovieCertification.Description)),
                    ReverseCountries = new MultiSelectList(dbCountryCodes, nameof(CountryCode.Code), nameof(CountryCode.Name)),
                    ReverseLanguages = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description)),
                    ReverseTranslations = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description)),
                    Name = list.Name,
                    Query = list.Query,
                    SearchByAlias = list.SearchByAlias,
                    SearchByBiography = list.SearchByBiography,
                    SearchByDescription = list.SearchByDescription,
                    SearchByName = list.SearchByName,
                    SearchByOverview = list.SearchByOverview,
                    SearchByPeople = list.SearchByPeople,
                    SearchByTitle = list.SearchByTitle,
                    SearchByTranslations = list.SearchByTranslations,
                    SearchByTagline = list.SearchByTagline,
                    Filter_Years = list.Filter_Years,
                    Filter_Runtimes = list.Filter_Runtimes,
                    Filter_Ratings_Trakt = list.Filter_Ratings_Trakt,
                    Filter_Ratings_IMDb = list.Filter_Ratings_IMDb,
                    Filter_Genres = list.Filter_Genres.Genres,
                    Filter_Certifications = list.Filter_Certifications_Movie.Certifications,
                    Filter_Countries = list.Filter_Countries.Languages,
                    Filter_Languages = list.Filter_Languages.Languages,
                    Filter_Translations = list.Filter_Translations.Translations,
                    ExclusionFilter_Genres = list.ExclusionFilter_Genres?.Genres,
                    ExclusionFilter_Certifications = list.ExclusionFilter_Certifications_Movie?.Certifications,
                    ExclusionFilter_Countries = list.ExclusionFilter_Countries?.Languages,
                    ExclusionFilter_Languages = list.ExclusionFilter_Languages?.Languages,
                    ExclusionFilter_Translations = list.ExclusionFilter_Translations?.Translations,
                    ExclusionFilter_Keywords = list.ExclusionFilter_Keywords == null ? string.Empty : string.Join(",", list.ExclusionFilter_Keywords)
                });
            }

            return RedirectToAction(nameof(My));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditMovieList(EditMovieListViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var list = await _traktRepository.Get(model.Id);
            if (list == null) return RedirectToAction(nameof(My));
            if (list.Type != ListType.Movie) return RedirectToAction(nameof(My));

            if (list.Owner.UserName == User.Identity.Name)
            {
                var forceRefresh = false;

                if (list.Name != model.Name)
                {
                    forceRefresh = true;
                }

                list.Name = model.Name;
                list.Query = model.Query ?? "";
                list.ContentType = ListContentType.Filters;
                list.SearchByAlias = model.SearchByAlias;
                list.SearchByBiography = model.SearchByBiography;
                list.SearchByDescription = model.SearchByDescription;
                list.SearchByName = model.SearchByName;
                list.SearchByOverview = model.SearchByOverview;
                list.SearchByPeople = model.SearchByPeople;
                list.SearchByTitle = model.SearchByTitle;
                list.SearchByTranslations = model.SearchByTranslations;
                list.SearchByTagline = model.SearchByTagline;
                list.Filter_Years = model.Filter_Years;
                list.Filter_Runtimes = model.Filter_Runtimes;
                list.Filter_Ratings_Trakt = model.Filter_Ratings_Trakt;
                list.Filter_Ratings_IMDb = model.Filter_Ratings_IMDb;
                list.Filter_Genres = new GenresCommonFilter(model.Filter_Genres);
                list.Filter_Languages = new LanguagesCommonFilter(model.Filter_Languages);
                list.Filter_Translations = new TranslationsBasicFilter(model.Filter_Translations);
                list.Filter_Certifications_Movie = new CertificationsMovieFilter(model.Filter_Certifications);
                list.Filter_Countries = new CountriesCommonFilter(model.Filter_Countries);

                if (list.Owner.IsDonor)
                {
                    list.ExclusionFilter_Genres = new GenresCommonFilter(model.ExclusionFilter_Genres);
                    list.ExclusionFilter_Languages = new LanguagesCommonFilter(model.ExclusionFilter_Languages);
                    list.ExclusionFilter_Translations = new TranslationsBasicFilter(model.ExclusionFilter_Translations);
                    list.ExclusionFilter_Certifications_Movie = new CertificationsMovieFilter(model.ExclusionFilter_Certifications);
                    list.ExclusionFilter_Countries = new CountriesCommonFilter(model.ExclusionFilter_Countries);
                    list.ExclusionFilter_Keywords = string.IsNullOrEmpty(model.ExclusionFilter_Keywords) ? null : model.ExclusionFilter_Keywords.Split(",");
                }

                await _traktService.Update(list);
                await _traktRepository.Update(list);

                if (list.ScanState == ScanState.None)
                    _backgroundJobQueueService.Queue(list, forceRefresh: forceRefresh);


                return RedirectToAction(nameof(EditMovieList), new { list.Id });
            }

            return RedirectToAction(nameof(My));
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ShowList()
        {
            ViewData["Message"] = "Create a new list for shows";

            var dbGenres = await _traktShowRepository.GetGenres();
            var dbStatus = await _traktShowRepository.GetStatuses();
            var dbNetworks = await _traktShowRepository.GetNetworks();
            var dbCertifications = await _traktShowRepository.GetCertifications();

            var dbCountryCodes = await _traktCodesRepository.GetCountryCodes();
            var dbLanguageCodes = await _traktCodesRepository.GetLanguageCodes();

            var model = new CreateShowListViewModel
            {
                Genres = new MultiSelectList(dbGenres, nameof(TraktShowGenre.Slug), nameof(TraktShowGenre.Name)),
                Certifications = new MultiSelectList(dbCertifications, nameof(TraktShowCertification.Slug), nameof(TraktShowCertification.Description)),
                Countries = new MultiSelectList(dbCountryCodes, nameof(CountryCode.Code), nameof(CountryCode.Name)),
                Languages = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description)),
                Translations = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description)),
                Networks = new MultiSelectList(dbNetworks, nameof(TraktShowNetwork.Name), nameof(TraktShowNetwork.Name)),
                Status = new MultiSelectList(dbStatus, nameof(TraktShowStatus.Name), nameof(TraktShowStatus.Name)),
                ReverseGenres = new MultiSelectList(dbGenres, nameof(TraktShowGenre.Slug), nameof(TraktShowGenre.Name)),
                ReverseCertifications = new MultiSelectList(dbCertifications, nameof(TraktShowCertification.Slug), nameof(TraktShowCertification.Description)),
                ReverseCountries = new MultiSelectList(dbCountryCodes, nameof(CountryCode.Code), nameof(CountryCode.Name)),
                ReverseLanguages = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description)),
                ReverseTranslations = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description)),
                ReverseNetworks = new MultiSelectList(dbNetworks, nameof(TraktShowNetwork.Name), nameof(TraktShowNetwork.Name)),
                ReverseStatus = new MultiSelectList(dbStatus, nameof(TraktShowStatus.Name), nameof(TraktShowStatus.Name)),
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ShowList(CreateShowListViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var lists = await _traktRepository.Get(user);

            if (lists.Count >= _limitConfigurationList.LimitConfigurations.First(x => x.Level == user.Level).ListLimit)
                return View("Error");

            var dbGenres = await _traktShowRepository.GetGenres();
            var dbStatus = await _traktShowRepository.GetStatuses();
            var dbNetworks = await _traktShowRepository.GetNetworks();
            var dbCertifications = await _traktShowRepository.GetCertifications();

            var dbCountryCodes = await _traktCodesRepository.GetCountryCodes();
            var dbLanguageCodes = await _traktCodesRepository.GetLanguageCodes();

            model.Genres = new MultiSelectList(dbGenres, nameof(TraktShowGenre.Slug), nameof(TraktShowGenre.Name));
            model.Certifications = new MultiSelectList(dbCertifications, nameof(TraktShowCertification.Slug), nameof(TraktShowCertification.Description));
            model.Countries = new MultiSelectList(dbCountryCodes, nameof(CountryCode.Code), nameof(CountryCode.Name));
            model.Languages = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description));
            model.Translations = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description));
            model.Networks = new MultiSelectList(dbNetworks, nameof(TraktShowNetwork.Name), nameof(TraktShowNetwork.Name));
            model.Status = new MultiSelectList(dbStatus, nameof(TraktShowStatus.Name), nameof(TraktShowStatus.Name));
            model.ReverseGenres = new MultiSelectList(dbGenres, nameof(TraktShowGenre.Slug), nameof(TraktShowGenre.Name));
            model.ReverseCertifications = new MultiSelectList(dbCertifications, nameof(TraktShowCertification.Slug), nameof(TraktShowCertification.Description));
            model.ReverseCountries = new MultiSelectList(dbCountryCodes, nameof(CountryCode.Code), nameof(CountryCode.Name));
            model.ReverseLanguages = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description));
            model.ReverseTranslations = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description));
            model.ReverseNetworks = new MultiSelectList(dbNetworks, nameof(TraktShowNetwork.Name), nameof(TraktShowNetwork.Name));
            model.ReverseStatus = new MultiSelectList(dbStatus, nameof(TraktShowStatus.Name), nameof(TraktShowStatus.Name));

            if (!ModelState.IsValid) return View(model);

            TraktList result = null;

            try
            {
                result = await _traktService.Create(new TraktList
                {
                    Name = model.Name,
                    Query = model.Query ?? "",
                    Type = ListType.Show,
                    Filter_Years = model.Filter_Years,
                    Filter_Ratings_Trakt = model.Filter_Ratings_Trakt,
                    Filter_Ratings_IMDb = model.Filter_Ratings_IMDb,
                    Filter_Runtimes = model.Filter_Runtimes,
                    SearchByAlias = model.SearchByAlias,
                    SearchByBiography = model.SearchByBiography,
                    SearchByDescription = model.SearchByDescription,
                    SearchByName = model.SearchByName,
                    SearchByOverview = model.SearchByOverview,
                    SearchByPeople = model.SearchByPeople,
                    SearchByTitle = model.SearchByTitle,
                    SearchByTranslations = model.SearchByTranslations,
                    Filter_Genres = new GenresCommonFilter(model.Filter_Genres),
                    Filter_Languages = new LanguagesCommonFilter(model.Filter_Languages),
                    Filter_Translations = new TranslationsBasicFilter(model.Filter_Translations),
                    Filter_Certifications_Show = new CertificationsShowFilter(model.Filter_Certifications),
                    Filter_Countries = new CountriesCommonFilter(model.Filter_Countries),
                    Filter_Networks = new NetworksShowFilter(model.Filter_Networks),
                    Filter_Status = new StatusShowFilter(model.Filter_Status),
                    Owner = user
                });
            }
            catch (TraktServerUnavailableException e)
            {
                return View(nameof(TraktServerUnavailableException));
            }

            if (user.IsDonor)
            {
                result.ExclusionFilter_Status = new StatusShowFilter(model.ExclusionFilter_Status);
                result.ExclusionFilter_Genres = new GenresCommonFilter(model.ExclusionFilter_Genres);
                result.ExclusionFilter_Networks = new NetworksShowFilter(model.ExclusionFilter_Networks);
                result.ExclusionFilter_Countries = new CountriesCommonFilter(model.ExclusionFilter_Countries);
                result.ExclusionFilter_Languages = new LanguagesCommonFilter(model.ExclusionFilter_Languages);
                result.ExclusionFilter_Translations = new TranslationsBasicFilter(model.ExclusionFilter_Translations);
                result.ExclusionFilter_Certifications_Show = new CertificationsShowFilter(model.ExclusionFilter_Certifications);
                result.ExclusionFilter_Keywords = string.IsNullOrEmpty(model.ExclusionFilter_Keywords) ? null : model.ExclusionFilter_Keywords.Split(",");
            }

            await _traktRepository.Create(result);

            _backgroundJobQueueService.Queue(result);

            return RedirectToAction(nameof(ShowList));
        }

        [HttpGet]
        [Authorize]
        public IActionResult ShowListFile()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ShowListFile(CreateMovieListFileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var lists = await _traktRepository.Get(user);

            if (lists.Count >= _limitConfigurationList.LimitConfigurations.First(x => x.Level == user.Level).ListLimit)
                return View("Error");

            if (!_limitConfigurationList.LimitConfigurations.First(x => x.Level == user.Level).ListsFromNames)
                return View("Error");

            if (!ModelState.IsValid) return View(model);

            var result = await _traktService.Create(new TraktList
            {
                Name = model.Name,
                Type = ListType.Show,
                ContentType = ListContentType.Names,
                ItemList = model.ItemList,
                Owner = user
            });

            await _traktRepository.Create(result);

            _backgroundJobQueueService.Queue(result);

            return RedirectToAction(nameof(ShowListFile));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditShowList(uint id)
        {
            ViewData["Message"] = "Create a new list for shows";

            var list = await _traktRepository.Get(id);
            if (list == null) return RedirectToAction(nameof(My));
            if (list.Type != ListType.Show) return RedirectToAction(nameof(My));

            if (list.Owner.UserName == User.Identity.Name)
            {
                var dbGenres = await _traktShowRepository.GetGenres();
                var dbStatus = await _traktShowRepository.GetStatuses();
                var dbNetworks = await _traktShowRepository.GetNetworks();
                var dbCertifications = await _traktShowRepository.GetCertifications();

                var dbCountryCodes = await _traktCodesRepository.GetCountryCodes();
                var dbLanguageCodes = await _traktCodesRepository.GetLanguageCodes();

                return View(new EditShowListViewModel
                {
                    Id = list.Id,
                    Genres = new MultiSelectList(dbGenres, nameof(TraktShowGenre.Slug), nameof(TraktShowGenre.Name)),
                    Certifications = new MultiSelectList(dbCertifications, nameof(TraktShowCertification.Slug), nameof(TraktShowCertification.Description)),
                    Countries = new MultiSelectList(dbCountryCodes, nameof(CountryCode.Code), nameof(CountryCode.Name)),
                    Languages = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description)),
                    Translations = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description)),
                    Networks = new MultiSelectList(dbNetworks, nameof(TraktShowNetwork.Name), nameof(TraktShowNetwork.Name)),
                    Status = new MultiSelectList(dbStatus, nameof(TraktShowStatus.Name), nameof(TraktShowStatus.Name)),
                    ReverseGenres = new MultiSelectList(dbGenres, nameof(TraktShowGenre.Slug), nameof(TraktShowGenre.Name)),
                    ReverseCertifications = new MultiSelectList(dbCertifications, nameof(TraktShowCertification.Slug), nameof(TraktShowCertification.Description)),
                    ReverseCountries = new MultiSelectList(dbCountryCodes, nameof(CountryCode.Code), nameof(CountryCode.Name)),
                    ReverseLanguages = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description)),
                    ReverseTranslations = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Description)),
                    ReverseNetworks = new MultiSelectList(dbNetworks, nameof(TraktShowNetwork.Name), nameof(TraktShowNetwork.Name)),
                    ReverseStatus = new MultiSelectList(dbStatus, nameof(TraktShowStatus.Name), nameof(TraktShowStatus.Name)),
                    Name = list.Name,
                    Query = list.Query,
                    SearchByAlias = list.SearchByAlias,
                    SearchByBiography = list.SearchByBiography,
                    SearchByDescription = list.SearchByDescription,
                    SearchByName = list.SearchByName,
                    SearchByOverview = list.SearchByOverview,
                    SearchByPeople = list.SearchByPeople,
                    SearchByTitle = list.SearchByTitle,
                    SearchByTranslations = list.SearchByTranslations,
                    Filter_Years = list.Filter_Years,
                    Filter_Runtimes = list.Filter_Runtimes,
                    Filter_Ratings_Trakt = list.Filter_Ratings_Trakt,
                    Filter_Ratings_IMDb = list.Filter_Ratings_IMDb,
                    Filter_Genres = list.Filter_Genres.Genres,
                    Filter_Certifications = list.Filter_Certifications_Show.Certifications,
                    Filter_Countries = list.Filter_Countries.Languages,
                    Filter_Languages = list.Filter_Languages.Languages,
                    Filter_Translations = list.Filter_Translations.Translations,
                    Filter_Networks = list.Filter_Networks.Networks,
                    Filter_Status = list.Filter_Status.Status,
                    ExclusionFilter_Genres = list.ExclusionFilter_Genres?.Genres,
                    ExclusionFilter_Certifications = list.ExclusionFilter_Certifications_Show?.Certifications,
                    ExclusionFilter_Countries = list.ExclusionFilter_Countries?.Languages,
                    ExclusionFilter_Languages = list.ExclusionFilter_Languages?.Languages,
                    ExclusionFilter_Translations = list.ExclusionFilter_Translations?.Translations,
                    ExclusionFilter_Networks = list.ExclusionFilter_Networks?.Networks,
                    ExclusionFilter_Status = list.ExclusionFilter_Status?.Status,
                    ExclusionFilter_Keywords = list.ExclusionFilter_Keywords == null ? string.Empty : string.Join(",", list.ExclusionFilter_Keywords)
                });
            }

            return RedirectToAction(nameof(My));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditShowList(EditShowListViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var list = await _traktRepository.Get(model.Id);
            if (list == null) return RedirectToAction(nameof(My));
            if (list.Type != ListType.Show) return RedirectToAction(nameof(My));

            if (list.Owner.UserName == User.Identity.Name)
            {
                var forceRefresh = false;

                if (list.Name != model.Name)
                {
                    forceRefresh = true;
                }

                list.Name = model.Name;
                list.Query = model.Query ?? "";
                list.SearchByAlias = model.SearchByAlias;
                list.SearchByBiography = model.SearchByBiography;
                list.SearchByDescription = model.SearchByDescription;
                list.SearchByName = model.SearchByName;
                list.SearchByOverview = model.SearchByOverview;
                list.SearchByPeople = model.SearchByPeople;
                list.SearchByTitle = model.SearchByTitle;
                list.SearchByTranslations = model.SearchByTranslations;
                list.Filter_Years = model.Filter_Years;
                list.Filter_Runtimes = model.Filter_Runtimes;
                list.Filter_Ratings_Trakt = model.Filter_Ratings_Trakt;
                list.Filter_Ratings_IMDb = model.Filter_Ratings_IMDb;
                list.Filter_Genres = new GenresCommonFilter(model.Filter_Genres);
                list.Filter_Languages = new LanguagesCommonFilter(model.Filter_Languages);
                list.Filter_Translations = new TranslationsBasicFilter(model.Filter_Translations);
                list.Filter_Certifications_Show = new CertificationsShowFilter(model.Filter_Certifications);
                list.Filter_Countries = new CountriesCommonFilter(model.Filter_Countries);
                list.Filter_Networks = new NetworksShowFilter(model.Filter_Networks);
                list.Filter_Status = new StatusShowFilter(model.Filter_Status);

                if (list.Owner.IsDonor)
                {
                    list.ExclusionFilter_Status = new StatusShowFilter(model.ExclusionFilter_Status);
                    list.ExclusionFilter_Genres = new GenresCommonFilter(model.ExclusionFilter_Genres);
                    list.ExclusionFilter_Networks = new NetworksShowFilter(model.ExclusionFilter_Networks);
                    list.ExclusionFilter_Countries = new CountriesCommonFilter(model.ExclusionFilter_Countries);
                    list.ExclusionFilter_Languages = new LanguagesCommonFilter(model.ExclusionFilter_Languages);
                    list.ExclusionFilter_Translations = new TranslationsBasicFilter(model.ExclusionFilter_Translations);
                    list.ExclusionFilter_Certifications_Show = new CertificationsShowFilter(model.ExclusionFilter_Certifications);
                    list.ExclusionFilter_Keywords = string.IsNullOrEmpty(model.ExclusionFilter_Keywords) ? null : model.ExclusionFilter_Keywords.Split(",");
                }

                await _traktService.Update(list);
                await _traktRepository.Update(list);

                if (list.ScanState == ScanState.None)
                    _backgroundJobQueueService.Queue(list, forceRefresh: forceRefresh);

                return RedirectToAction(nameof(EditShowList), new { list.Id });
            }

            return RedirectToAction(nameof(My));
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(uint id)
        {
            var list = await _traktRepository.Get(id);
            if (list == null) return RedirectToAction(nameof(My));

            if (list.Owner.UserName == User.Identity.Name)
            {
                return View(new DeleteListViewModel
                {
                    Id = list.Id,
                    Items = list.Items,
                    Name = list.Name
                });
            }

            return RedirectToAction(nameof(My));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(DeleteListViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var list = await _traktRepository.Get(model.Id);
            if (list == null) return RedirectToAction(nameof(My));

            if (list.Owner.UserName == User.Identity.Name)
            {
                try
                {
                    await _traktService.Delete(list);
                }
                catch (TraktListNotFoundException)
                {
                }

                await _traktRepository.Delete(list);
            }

            return RedirectToAction(nameof(My));
        }
    }
}