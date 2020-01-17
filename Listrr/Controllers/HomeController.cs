using System.Diagnostics;
using System.Threading.Tasks;

using Listrr.Configuration;
using Listrr.Data;
using Listrr.Models;
using Listrr.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Listrr.Controllers
{
    public class HomeController : Controller
    {

        private readonly ITraktService _traktService;
        private readonly ToplistConfiguration _toplistConfiguration;

        public HomeController(ITraktService traktService, ToplistConfiguration toplistConfiguration)
        {
            _traktService = traktService;
            _toplistConfiguration = toplistConfiguration;
        }


        public async Task<IActionResult> Index()
        {
            var lists = await _traktService.Top(_toplistConfiguration.Count, _toplistConfiguration.Threshold);

            return View(lists);
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
