using Hangfire;

using HangfireBasicAuthenticationFilter;
using Listrr.Configuration;
using Listrr.Data;
using Listrr.Repositories;
using Listrr.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Listrr.Jobs.RecurringJobs;

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

            // Config
            var hangfireConfiguration = new HangFireConfiguration();
            Configuration.Bind("Hangfire", hangfireConfiguration);
            services.AddSingleton(hangfireConfiguration);

            var toplistConfiguration = new ToplistConfiguration();
            Configuration.Bind("Toplist", toplistConfiguration);
            services.AddSingleton(toplistConfiguration);

            var traktApiConfiguration = new TraktAPIConfiguration();
            Configuration.Bind("Trakt", traktApiConfiguration);
            services.AddSingleton(traktApiConfiguration);

            // Multi Instance LB
            services.AddDbContext<DataProtectionDbContext>(options =>
                options.UseSqlServer(connectionString)
            );

            services.AddDataProtection()
                .PersistKeysToDbContext<DataProtectionDbContext>()
                .SetApplicationName("Listrr");

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = connectionString;
                options.SchemaName = "dbo";
                options.TableName = "Cache";
            });


            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString)
            );

            services.AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.User.AllowedUserNameCharacters = null;
                }).AddEntityFrameworkStores<AppDbContext>();

            services.AddHangfire(x =>
                x.UseSqlServerStorage(connectionString));

            services.AddAuthentication()
                .AddTrakt(options =>
                {
                    options.ClientId = traktApiConfiguration.ClientId;
                    options.ClientSecret = traktApiConfiguration.ClientSecret;
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

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.Name = "Listrr";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(1);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddHttpContextAccessor();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            
            services.AddScoped<ITraktListDBRepository, TraktListDBRepository>();
            services.AddScoped<ITraktListAPIRepository, TraktListAPIRepository>();
            services.AddScoped<ITraktService, TraktService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, HangFireConfiguration hangFireConfiguration)
        {
            InitializeDatabase(app);

            app.UseForwardedHeaders();

            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";
                return next();
            });

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

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                WorkerCount = hangFireConfiguration.Workers
            });

            app.UseHangfireDashboard(hangFireConfiguration.DashboardPath ?? "/jobs", new DashboardOptions
            {
                Authorization = new[] { new HangfireCustomBasicAuthenticationFilter { User = hangFireConfiguration.Username ?? "Admin", Pass = hangFireConfiguration.Password ?? "SuperSecurePWD!123" } }
            });

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
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }


        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<DataProtectionDbContext>().Database.Migrate();
            }
        }
    }
}