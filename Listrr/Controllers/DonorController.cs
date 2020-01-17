using System.Linq;
using System.Threading.Tasks;
using Listrr.Configuration;
using Listrr.Data;
using Listrr.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Listrr.Controllers
{
    public class DonorController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly DonorConfiguration _donorConfiguration;

        public DonorController(AppDbContext appDbContext, DonorConfiguration donorConfiguration)
        {
            _appDbContext = appDbContext;
            _donorConfiguration = donorConfiguration;
        }
        


        //public IActionResult LinkGitHubAccount()
        //{
        //    //return redirectto
        //}

        public async Task<IActionResult> WhyDonate()
        {
            var lists = await _appDbContext.TraktLists.Include(x => x.Owner).ToListAsync();
            var listsWithExclusionFilters = lists.Where(x =>
                x.ReverseFilter_Certifications_Movie?.Certifications?.Length > 0 ||
                x.ReverseFilter_Certifications_Show?.Certifications?.Length > 0 ||
                x.ReverseFilter_Countries?.Languages?.Length > 0 ||
                x.ReverseFilter_Genres?.Genres?.Length > 0 ||
                x.ReverseFilter_Languages?.Languages?.Length > 0 ||
                x.ReverseFilter_Networks?.Networks?.Length > 0 ||
                x.ReverseFilter_Status?.Status?.Length > 0 ||
                x.ReverseFilter_Translations?.Translations?.Length > 0
            ).ToList();
            
            return View(new WhyDonateViewModel()
            {
                Lists = lists.Count,
                Users = await _appDbContext.Users.CountAsync(),
                ListsWithExclusionFilters = listsWithExclusionFilters.Count(),
                UsersWithExclusionFilters = listsWithExclusionFilters.Select(x => x.Owner).Distinct().Count(),
                UserListLimit = _donorConfiguration.ListLimit
            });
        }
    }
}