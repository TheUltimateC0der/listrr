using System;
using System.Collections.Generic;
using System.Text;
using Listrr.API.Trakt.Models.Filters;
using Listrr.Data.Trakt;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Listrr.Data
{
    public class AppDbContext : IdentityDbContext
    {

        public DbSet<TraktList> TraktLists { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<TraktList>()
                .Property(x => x.Filter_Countries)
                .HasConversion(
                    x => string.Join(",", x.Languages),
                    x => new CountriesCommonFilter(x)
                );

            builder
                .Entity<TraktList>()
                .Property(x => x.Filter_Certifications)
                .HasConversion(
                    x => string.Join(",", x.Certifications),
                    x => new CertificationsMovieFilter(x)
                );

            builder
                .Entity<TraktList>()
                .Property(x => x.Filter_Genres)
                .HasConversion(
                    x => string.Join(",", x.Genres),
                    x => new GenresCommonFilter(x)
                );

            builder
                .Entity<TraktList>()
                .Property(x => x.Filter_Languages)
                .HasConversion(
                    x => string.Join(",", x.Languages),
                    x => new LanguagesCommonFilter(x)
                );

            builder
                .Entity<TraktList>()
                .Property(x => x.Filter_Ratings)
                .HasConversion(
                    x => $"{x.From}-{x.To}",
                    x => new RatingsCommonFilter(x)
                );
            
            builder
                .Entity<TraktList>()
                .Property(x => x.Filter_Runtimes)
                .HasConversion(
                    x => $"{x.From}-{x.To}",
                    x => new RuntimesCommonFilter(x)
                );

            builder
                .Entity<TraktList>()
                .Property(x => x.Filter_Years)
                .HasConversion(
                    x => $"{x.From}-{x.To}",
                    x => new YearsCommonFilter(x)
                );


            base.OnModelCreating(builder);
        }
    }
}