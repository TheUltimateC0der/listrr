using System.Diagnostics;
using System.Threading.Tasks;

using Listrr.Configuration;
using Listrr.Data.Trakt;
using Listrr.Models;
using Listrr.Repositories;

using Microsoft.AspNetCore.Mvc;

namespace Listrr.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITraktListRepository _traktListRepository;
        private readonly ListPaginationConfiguration _listPaginationConfiguration;

        public HomeController(ITraktListRepository traktListRepository, ListPaginationConfiguration listPaginationConfiguration)
        {
            _traktListRepository = traktListRepository;
            _listPaginationConfiguration = listPaginationConfiguration;
        }


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Lists(int id)
        {
            var listCount = await _traktListRepository.Count();
            var pageCount = (listCount + 9) / _listPaginationConfiguration.PageSize;

            if (id > pageCount) return RedirectToAction("Error");
            if (id < 0) return RedirectToAction("Error");

            var lists = await _traktListRepository.Get(_listPaginationConfiguration.PageSize, _listPaginationConfiguration.PageSize * id + (id != 0 ? 1 : 0));

            return View(new PaginationViewModel<TraktList>
            {
                Items = lists, 
                Pages = pageCount, 
                Page = id
            });
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
