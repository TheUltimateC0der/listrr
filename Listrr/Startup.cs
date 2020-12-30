using Hangfire;
using Hangfire.Console;
using Hangfire.Dashboard.Dark;

using HangfireBasicAuthenticationFilter;

using Listrr.Configuration;
using Listrr.Data;
using Listrr.Jobs.RecurringJobs;
using Listrr.Jobs.RecurringJobs.IMDb;
using Listrr.Repositories;
using Listrr.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using MoreLinq;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Listrr
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            // Config
            var hangfireConfiguration = new HangFireConfiguration();
            Configuration.Bind("Hangfire", hangfireConfiguration);
            services.AddSingleton(hangfireConfiguration);

            var listPaginationConfiguration = new ListPaginationConfiguration();
            Configuration.Bind("ListPagination", listPaginationConfiguration);
            services.AddSingleton(listPaginationConfiguration);

            var traktApiConfiguration = new TraktAPIConfiguration();
            Configuration.Bind("Trakt", traktApiConfiguration);
            services.AddSingleton(traktApiConfiguration);

            var githubApiConfiguration = new GithubAPIConfiguration();
            Configuration.Bind("GitHub", githubApiConfiguration);
            services.AddSingleton(githubApiConfiguration);

            var discordApiConfiguration = new DiscordAPIConfiguration();
            Configuration.Bind("Discord", discordApiConfiguration);
            services.AddSingleton(discordApiConfiguration);

            var limitConfigurationList = new LimitConfigurationList();
            Configuration.Bind("LimitConfig", limitConfigurationList);
            services.AddSingleton(limitConfigurationList);

            var userMappingConfigurationList = new UserMappingConfigurationList();
            Configuration.Bind("UserMappingConfig", userMappingConfigurationList);
            services.AddSingleton(userMappingConfigurationList);

            var notificationConfiguration = new NotificationConfiguration();
            Configuration.Bind("Notification", notificationConfiguration);
            services.AddSingleton(notificationConfiguration);


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
            services.AddDefaultIdentity<User>(options =>
            {
                options.User.AllowedUserNameCharacters = null;
            }).AddEntityFrameworkStores<AppDbContext>();
            services.AddHangfire(x =>
            {
                x.UseSqlServerStorage(connectionString)
                    .WithJobExpirationTimeout(TimeSpan.FromHours(24));

                x.UseConsole();
            });

            services.AddAuthentication()
                .AddTrakt(options =>
                {
                    options.ClientId = traktApiConfiguration.ClientId;
                    options.ClientSecret = traktApiConfiguration.ClientSecret;
                    options.SaveTokens = true;

                    options.ClaimActions.MapJsonSubKey(Constants.Trakt_Claim_Ids_Slug, "ids", "slug");

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
                })
                .AddGitHub(options =>
                {
                    options.ClientId = githubApiConfiguration.ClientId;
                    options.ClientSecret = githubApiConfiguration.ClientSecret;
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
                })
                .AddDiscord(options =>
                {
                    options.ClientId = discordApiConfiguration.ClientId;
                    options.ClientSecret = discordApiConfiguration.ClientSecret;
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

            services.AddHttpContextAccessor();

            //ReverseProxy Fix
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddScoped<IGitHubGraphService, GitHubGraphService>();
            services.AddScoped<IBackgroundJobQueueService, BackgroundJobQueueService>();
            services.AddScoped<ITraktListRepository, TraktListRepository>();
            services.AddScoped<ITraktMovieRepository, TraktMovieRepository>();
            services.AddScoped<ITraktShowRepository, TraktShowRepository>();
            services.AddScoped<ITraktCodeRepository, TraktCodeRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITraktService, TraktService>();
            services.AddScoped<IIMDbRepository, IMDbRepository>();

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, HangFireConfiguration hangFireConfiguration, LimitConfigurationList limitConfigurationList)
        {
            InitializeDatabase(app);

            //ReverseProxy Fix
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

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            InitializeHangfire(app, serviceProvider, hangFireConfiguration, limitConfigurationList);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHangfireDashboard(new DashboardOptions
                {
                    Authorization = new[]
                    {
                        new HangfireCustomBasicAuthenticationFilter
                        {
                            User = hangFireConfiguration.Username ?? "Admin",
                            Pass = hangFireConfiguration.Password ?? "SuperSecurePWD!123"
                        }
                    }
                });
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            serviceScope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
            serviceScope.ServiceProvider.GetRequiredService<DataProtectionDbContext>().Database.Migrate();
        }

        private void InitializeHangfire(IApplicationBuilder app, IServiceProvider serviceProvider, HangFireConfiguration hangFireConfiguration, LimitConfigurationList limitConfigurationList)
        {
            GlobalConfiguration.Configuration.UseActivator(new HangfireActivator(serviceProvider));
            GlobalConfiguration.Configuration.UseDarkDashboard();

            var queues = new List<string> { "system" };

            foreach (var limitConfiguration in limitConfigurationList.LimitConfigurations.DistinctBy(x => x.QueueName))
            {
                queues.Add(limitConfiguration.QueueName);
            }

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                WorkerCount = hangFireConfiguration.Workers,
                Queues = queues.ToArray()
            });

            app.UseHangfireDashboard(hangFireConfiguration.DashboardPath ?? "/jobs", new DashboardOptions
            {
                Authorization = new[] { new HangfireCustomBasicAuthenticationFilter
                {
                    User = hangFireConfiguration.Username ?? "Admin",
                    Pass = hangFireConfiguration.Password ?? "SuperSecurePWD!123"
                } }
            });

            RecurringJob.AddOrUpdate<GetMovieCertificationsRecurringJob>(x => x.Execute(), Cron.Daily);
            RecurringJob.AddOrUpdate<GetShowCertificationsRecurringJob>(x => x.Execute(), Cron.Daily);
            RecurringJob.AddOrUpdate<GetMovieGenresRecurringJob>(x => x.Execute(), Cron.Daily);
            RecurringJob.AddOrUpdate<GetShowGenresRecurringJob>(x => x.Execute(), Cron.Daily);
            RecurringJob.AddOrUpdate<GetCountryCodesRecurringJob>(x => x.Execute(), Cron.Daily);
            RecurringJob.AddOrUpdate<GetLanguageCodesRecurringJob>(x => x.Execute(), Cron.Daily);
            RecurringJob.AddOrUpdate<GetShowNetworksRecurringJob>(x => x.Execute(), Cron.Daily);
            RecurringJob.AddOrUpdate<GetShowStatusRecurringJob>(x => x.Execute(), Cron.Daily);

            RecurringJob.AddOrUpdate<ProcessDonorListsRecurringJob>(x => x.Execute(), Cron.Daily);
            RecurringJob.AddOrUpdate<ProcessUserListsRecurringJob>(x => x.Execute(), Cron.Never);
            RecurringJob.AddOrUpdate<UpdateAllListsRecurringJob>(x => x.Execute(), Cron.Never);

            RecurringJob.AddOrUpdate<EnforceListLimitRecurringJob>(x => x.Execute(), "*/5 * * * *");
            RecurringJob.AddOrUpdate<SetDonorsRecurringJob>(x => x.Execute(), "*/5 * * * *");

            RecurringJob.AddOrUpdate<IMDbRatingsRecurringJob>(x => x.Execute(), "0 3 * * *");

            //BackgroundJob.Enqueue<ProcessMovieListBackgroundJob>(x => x.ExecutePriorized(XXXXXXXXX, null, false, false));

            ////Starting all jobs here for initial db fill
            //foreach (var recurringJob in JobStorage.Current.GetConnection().GetRecurringJobs())
            //{
            //    RecurringJob.Trigger(recurringJob.Id);
            //}
        }

    }
}