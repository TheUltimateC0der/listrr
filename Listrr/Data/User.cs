using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

namespace Listrr.Data
{

    public enum UserLevel
    {
        User,
        Donor,
    }

    public class User : IdentityUser
    {

        

        [NotMapped]
        public bool IsDonor => Level != UserLevel.User;

        public UserLevel Level { get; set; }
        
    }
}