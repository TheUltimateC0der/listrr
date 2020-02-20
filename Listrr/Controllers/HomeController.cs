using Listrr.Configuration;
using Listrr.Models;
using Listrr.Repositories;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using System.Threading.Tasks;

namespace Listrr.Controllers
{
    public class HomeController : Controller
    {

        private readonly ITraktListRepository _traktRepository;
        private readonly ToplistConfiguration _toplistConfiguration;

        public HomeController(ToplistConfiguration toplistConfiguration, ITraktListRepository traktRepository)
        {
            _toplistConfiguration = toplistConfiguration;
            _traktRepository = traktRepository;
        }


        public async Task<IActionResult> Index()
        {
            var lists = await _traktRepository.Top(_toplistConfiguration.Count, _toplistConfiguration.Threshold);

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
