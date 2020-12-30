using Listrr.Data.Trakt;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .Property(x => x.Level)
                .HasDefaultValue(UserLevel.User);

            builder
                .HasMany<TraktList>(x => x.TraktLists)
                .WithOne(x => x.Owner)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}