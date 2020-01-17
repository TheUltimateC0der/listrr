using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;

namespace Listrr.Data
{
    public class User : IdentityUser
    {

        public enum UserLevel
        {
            User,
            Donor,
        }

        [NotMapped]
        public bool IsDonor => Level != UserLevel.User;

        public UserLevel Level { get; set; }
        
    }
}