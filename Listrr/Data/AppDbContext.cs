using System;
using System.Collections.Generic;
using System.Text;
using Listrr.API.Trakt.Models.Filters;
using Listrr.Data.Trakt;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TraktNet.Enums;

namespace Listrr.Data
{
    public class AppDbContext : IdentityDbContext
    {

        public DbSet<TraktList> TraktLists { get; set; }

        public DbSet<TraktMovieCertification> TraktMovieCertifications { get; set; }
        public DbSet<TraktShowCertification> TraktShowCertifications { get; set; }
        public DbSet<TraktShowGenre> TraktShowGenres { get; set; }
        public DbSet<TraktMovieGenre> TraktMovieGenres { get; set; }


        public DbSet<CountryCode> CountryCodes { get; set; }
        public DbSet<LanguageCode> LanguageCodes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {

            #region TraktShowGenre

            builder
                .Entity<TraktShowGenre>()
                .HasIndex(x => x.Slug)
                .IsUnique();

            builder
                .Entity<TraktShowGenre>()
                .HasKey(x => x.Slug);

            #endregion

            #region TraktMovieGenre

            builder
                .Entity<TraktMovieGenre>()
                .HasIndex(x => x.Slug)
                .IsUnique();

            builder
                .Entity<TraktMovieGenre>()
                .HasKey(x => x.Slug);

            #endregion

            #region TraktMovieCertification

            builder
                .Entity<TraktMovieCertification>()
                .HasIndex(x => x.Slug)
                .IsUnique();

            builder
                .Entity<TraktMovieCertification>()
                .HasKey(x => x.Slug);

            #endregion

            #region TraktShowCertification

            builder
                .Entity<TraktShowCertification>()
                .HasIndex(x => x.Slug)
                .IsUnique();

            builder
                .Entity<TraktShowCertification>()
                .HasKey(x => x.Slug);

            #endregion

            #region TraktList

            builder
                .Entity<TraktList>()
                .HasIndex(x => new { x.Slug, x.Name })
                .IsUnique();

            builder
                .Entity<TraktList>()
                .Property(x => x.Filter_Countries)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<CountriesCommonFilter>(x)
                );

            builder
                .Entity<TraktList>()
                .Property(x => x.Filter_Certifications)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<CertificationsMovieFilter>(x)
                );

            builder
                .Entity<TraktList>()
                .Property(x => x.Filter_Genres)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<GenresCommonFilter>(x)
                );

            builder
                .Entity<TraktList>()
                .Property(x => x.Filter_Languages)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<LanguagesCommonFilter>(x)
                );

            builder
                .Entity<TraktList>()
                .Property(x => x.Filter_Ratings)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<RatingsCommonFilter>(x)
                );

            builder
                .Entity<TraktList>()
                .Property(x => x.Filter_Runtimes)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<RuntimesCommonFilter>(x)
                );

            builder
                .Entity<TraktList>()
                .Property(x => x.Filter_Years)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<YearsCommonFilter>(x)
                );

            builder
                .Entity<TraktList>()
                .Property(x => x.Filter_Translations)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<TranslationsBasicFilter>(x)
                );

            builder
                .Entity<TraktList>()
                .Property(x => x.Filter_SearchField)
                .HasConversion(
                    x => JsonConvert.SerializeObject(x),
                    x => JsonConvert.DeserializeObject<TraktSearchField>(x)
                );

            #endregion

            #region CountryCode

            builder
                .Entity<CountryCode>()
                .HasIndex(x => x.Code);

            builder
                .Entity<CountryCode>()
                .HasKey(x => new {x.Name, x.Code});

            #endregion

            #region LanguageCode

            builder
                .Entity<LanguageCode>()
                .HasIndex(x => x.Code);

            builder
                .Entity<LanguageCode>()
                .HasKey(x => new { x.Name, x.Code });

            #endregion



            base.OnModelCreating(builder);
        }
    }
}