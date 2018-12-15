using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Listrr.Models;
using Listrr.Repositories;
using Listrr.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Listrr.Controllers
{
    public class HomeController : Controller
    {

        private readonly ITraktService traktService;
        private readonly UserManager<IdentityUser> userManager;

        public HomeController(ITraktService traktService, UserManager<IdentityUser> userManager)
        {
            this.traktService = traktService;
            this.userManager = userManager;
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
        public IActionResult MovieList()
        {
            ViewData["Message"] = "Create a new list for movies";

            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult MovieList(CreateMovieListViewModel model)
        {
            ViewData["Message"] = "Create a new list for movies";

            if (!ModelState.IsValid) return View(model);
            



            return View();
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
