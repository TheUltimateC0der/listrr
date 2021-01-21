using Listrr.Data.IMDb;
using Listrr.Data.Trakt;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Listrr.Data
{
    public class MsSQLDbContext : IdentityDbContext<User>
    {

        public DbSet<TraktList> TraktLists { get; set; }

        public DbSet<TraktMovieCertification> TraktMovieCertifications { get; set; }
        public DbSet<TraktShowCertification> TraktShowCertifications { get; set; }
        public DbSet<TraktShowGenre> TraktShowGenres { get; set; }
        public DbSet<TraktMovieGenre> TraktMovieGenres { get; set; }


        public DbSet<TraktShowNetwork> TraktShowNetworks { get; set; }
        public DbSet<TraktShowStatus> TraktShowStatuses { get; set; }


        public DbSet<CountryCode> CountryCodes { get; set; }
        public DbSet<LanguageCode> LanguageCodes { get; set; }

        public DbSet<IMDbRating> ImDbRatings { get; set; }

        public MsSQLDbContext(DbContextOptions<MsSQLDbContext> options) : base(options)
        {
            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }



        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is CreatedAndUpdated && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((CreatedAndUpdated)entityEntry.Entity).Updated = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    ((CreatedAndUpdated)entityEntry.Entity).Created = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is CreatedAndUpdated && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((CreatedAndUpdated)entityEntry.Entity).Updated = DateTime.UtcNow;

                if (entityEntry.State == EntityState.Added)
                {
                    ((CreatedAndUpdated)entityEntry.Entity).Created = DateTime.UtcNow;
                }
            }

            return base.SaveChanges();
        }
    }
}