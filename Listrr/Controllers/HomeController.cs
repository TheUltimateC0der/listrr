using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Listrr.API.Trakt.Models.Filters;
using Listrr.Data;
using Listrr.Data.Trakt;
using Microsoft.AspNetCore.Mvc;
using Listrr.Models;
using Listrr.Repositories;
using Listrr.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TraktNet;
using TraktNet.Enums;
using TraktNet.Objects.Authentication;

namespace Listrr.Controllers
{
    public class HomeController : Controller
    {

        private readonly ITraktService traktService;
        private readonly UserManager<IdentityUser> userManager;
        private readonly AppDbContext appDbContext;

        public HomeController(ITraktService traktService, UserManager<IdentityUser> userManager, AppDbContext appDbContext)
        {
            this.traktService = traktService;
            this.userManager = userManager;
            this.appDbContext = appDbContext;
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

            return View(await traktService.Get(await userManager.GetUserAsync(User)));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MovieList()
        {
            ViewData["Message"] = "Create a new list for movies";

            var dbGenres = await appDbContext.TraktMovieGenres.ToListAsync();
            var dbCertifications = await appDbContext.TraktMovieCertifications.ToListAsync();
            var dbCountryCodes = await appDbContext.CountryCodes.OrderBy(x => x.Name).ToListAsync();
            var dbLanguageCodes = await appDbContext.LanguageCodes.OrderBy(x => x.Name).ToListAsync();
            
            var model = new CreateMovieListViewModel()
            {
                Genres = new MultiSelectList(dbGenres, nameof(TraktMovieGenre.Slug), nameof(TraktMovieGenre.Slug)),
                Certifications = new MultiSelectList(dbCertifications, nameof(TraktMovieCertification.Slug), nameof(TraktMovieCertification.Description)),
                Countries = new MultiSelectList(dbCountryCodes, nameof(CountryCode.Code), nameof(CountryCode.Name)),
                Languages = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Name)),
                Translations = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Name))
            };

            var test = TraktSearchField.Name | TraktSearchField.Biography;

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> MovieList(CreateMovieListViewModel model)
        {
            ViewData["Message"] = "Create a new list for movies";

            var dbGenres = await appDbContext.TraktMovieGenres.ToListAsync();
            var dbCertifications = await appDbContext.TraktMovieCertifications.ToListAsync();
            var dbCountryCodes = await appDbContext.CountryCodes.OrderBy(x => x.Name).ToListAsync();
            var dbLanguageCodes = await appDbContext.LanguageCodes.OrderBy(x => x.Name).ToListAsync();

            model.Genres = new MultiSelectList(dbGenres, nameof(TraktMovieGenre.Slug), nameof(TraktMovieGenre.Slug));
            model.Certifications = new MultiSelectList(dbCertifications, nameof(TraktMovieCertification.Slug), nameof(TraktMovieCertification.Description));
            model.Countries = new MultiSelectList(dbCountryCodes, nameof(CountryCode.Code), nameof(CountryCode.Name));
            model.Languages = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Name));
            model.Translations = new MultiSelectList(dbLanguageCodes, nameof(LanguageCode.Code), nameof(LanguageCode.Name));

            //if (!ModelState.IsValid) return BadRequest(ModelState);
            if (!ModelState.IsValid) return View(model);

            var certifications = model.Filter_Certifications != null ? string.Join(',', model.Filter_Certifications) : "";
            var countries = model.Filter_Countries != null ? string.Join(',', model.Filter_Countries) : "";
            var genres = model.Filter_Genres != null ? string.Join(',', model.Filter_Genres) : "";
            var languages = model.Filter_Languages != null ? string.Join(',', model.Filter_Languages) : "";
            var translations = model.Filter_Translations != null ? string.Join(',', model.Filter_Translations) : "";
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
            
            await traktService.Create(new TraktList()
            {
                Name = model.Name,
                Query = model.Query ?? "",
                Filter_SearchField = searchFields,
                Filter_Years = model.Filter_Years,
                Filter_Ratings = model.Filter_Ratings,
                Filter_Runtimes = model.Filter_Runtimes,
                Filter_Genres = new GenresCommonFilter(genres),
                Filter_Languages = new LanguagesCommonFilter(languages),
                Filter_Translations = new TranslationsBasicFilter(translations),
                Filter_Certifications = new CertificationsMovieFilter(certifications),
                Filter_Countries = new CountriesCommonFilter(countries),
                Owner = await userManager.GetUserAsync(User)
            });

            return RedirectToAction(nameof(MovieList));
        }

        [HttpGet]
        [Authorize]
        public IActionResult ShowList()
        {
            ViewData["Message"] = "Overview of your lists";

            return View();
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
