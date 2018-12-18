using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Listrr.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using Listrr.BackgroundJob;
using Listrr.Repositories;
using Listrr.Services;
using Microsoft.AspNetCore.Authentication;

namespace Listrr
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString)
            );

            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddHangfire(x =>
                x.UseSqlServerStorage(connectionString));

            services.AddAuthentication()
                .AddTrakt(options =>
                {
                    options.ClientId = Configuration["Trakt:ClientID"];
                    options.ClientSecret = Configuration["Trakt:ClientSecret"];
                    options.SaveTokens = true;
                    options.Events.OnCreatingTicket = ctx =>
                    {
                        List<AuthenticationToken> tokens = ctx.Properties.GetTokens() as List<AuthenticationToken>;
                        tokens.Add(new AuthenticationToken()
                        {
                            Name = "TicketCreated",
                            Value = DateTime.Now.ToString()
                        });
                        ctx.Properties.StoreTokens(tokens);
                        return Task.CompletedTask;
                    };
                });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<AppDbContext>(options => 
                options.UseSqlServer(connectionString));

            services.AddHttpContextAccessor();

            services.AddScoped<ITraktListDBRepository, TraktListDBRepository>();
            services.AddScoped<ITraktListAPIRepository, TraktListAPIRepository>();
            services.AddScoped<ITraktService, TraktService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            GlobalConfiguration.Configuration
                .UseActivator(new HangfireActivator(serviceProvider));

            app.UseHangfireServer();
            if(env.IsDevelopment()) //Check this, couse reverseproxy could fuckup the "IsLocalhost" request
                app.UseHangfireDashboard();

            RecurringJob.AddOrUpdate<GetMovieCertificationsRecurringJob>((x) => x.Execute(), Cron.Daily);
            RecurringJob.AddOrUpdate<GetShowCertificationsRecurringJob>((x) => x.Execute(), Cron.Daily);
            RecurringJob.AddOrUpdate<GetMovieGenresRecurringJob>((x) => x.Execute(), Cron.Daily);
            RecurringJob.AddOrUpdate<GetShowGenresRecurringJob>((x) => x.Execute(), Cron.Daily);
            RecurringJob.AddOrUpdate<GetCountryCodesRecurringJob>((x) => x.Execute(), Cron.Daily);
            RecurringJob.AddOrUpdate<GetLanguageCodesRecurringJob>((x) => x.Execute(), Cron.Daily);
            RecurringJob.AddOrUpdate<ProcessListsRecurringJob>((x) => x.Execute(), Cron.Daily);
            RecurringJob.AddOrUpdate<GetShowNetworksRecurringJob>((x) => x.Execute(), Cron.Daily);
            RecurringJob.AddOrUpdate<GetShowStatusRecurringJob>((x) => x.Execute(), Cron.Daily);


            ////Starting all jobs here for initial db fill
            //foreach (var recurringJob in JobStorage.Current.GetConnection().GetRecurringJobs())
            //{
            //    RecurringJob.Trigger(recurringJob.Id);
            //}

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Lists}/{id?}");
            });
        }
    }
}