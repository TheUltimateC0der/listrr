using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Listrr.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        public IActionResult OnGet() => RedirectToPage("/Account/Login");
    }
}
