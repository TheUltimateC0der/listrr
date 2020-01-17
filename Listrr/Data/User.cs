using Microsoft.AspNetCore.Identity;

namespace Listrr.Data
{
    public class User : IdentityUser
    {

        public bool IsDonor { get; set; }


    }
}