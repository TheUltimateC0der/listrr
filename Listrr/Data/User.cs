using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Listrr.Data.Trakt;

using Microsoft.AspNetCore.Identity;

namespace Listrr.Data
{

    public enum UserLevel
    {
        User,
        Donor,
        DonorPlus,
        DonorPlusPlus,
        Developer
    }

    public class User : IdentityUser
    {

        [NotMapped]
        public bool IsDonor => Level != UserLevel.User;

        public UserLevel Level { get; set; }

        public IList<TraktList> TraktLists { get; set; }
        
    }
}