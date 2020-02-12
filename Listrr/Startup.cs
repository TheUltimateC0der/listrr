using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Hangfire;

using HangfireBasicAuthenticationFilter;

using Listrr.Configuration;
using Listrr.Data;
using Listrr.Jobs.RecurringJobs;
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

            var githubApiConfiguration = new GithubAPIConfiguration();
            Configuration.Bind("GitHub", githubApiConfiguration);
            services.AddSingleton(githubApiConfiguration);

            var limitConfigurationList = new LimitConfigurationList();
            Configuration.Bind("LimitConfig", limitConfigurationList);
            services.AddSingleton(limitConfigurationList);

            var userMappingConfigurationList = new UserMappingConfigurationList();
            Configuration.Bind("UserMappingConfig", userMappingConfigurationList);
            services.AddSingleton(userMappingConfigurationList);


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
            services.AddScoped<ITraktListDBRepository, TraktListDBRepository>();
            services.AddScoped<ITraktListAPIRepository, TraktListAPIRepository>();
            services.AddScoped<ITraktService, TraktService>();

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
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
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
                serviceScope.ServiceProvider.GetRequiredService<DataProtectionDbContext>().Database.Migrate();
            }
        }

        private void InitializeHangfire(IApplicationBuilder app, IServiceProvider serviceProvider, HangFireConfiguration hangFireConfiguration, LimitConfigurationList limitConfigurationList)
        {
            GlobalConfiguration.Configuration
                .UseActivator(new HangfireActivator(serviceProvider));

            var queues = new List<string>();
            queues.Add("system");

            foreach (var limitConfiguration in limitConfigurationList.LimitConfigurations)
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
                Authorization = new[] { new HangfireCustomBasicAuthenticationFilter { User = hangFireConfiguration.Username ?? "Admin", Pass = hangFireConfiguration.Password ?? "SuperSecurePWD!123" } }
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

            RecurringJob.AddOrUpdate<EnforceListLimitRecurringJob>(x => x.Execute(), "*/5 * * * *");
            RecurringJob.AddOrUpdate<SetDonorsRecurringJob>(x => x.Execute(), "*/5 * * * *");
            

            ////Starting all jobs here for initial db fill
            //foreach (var recurringJob in JobStorage.Current.GetConnection().GetRecurringJobs())
            //{
            //    RecurringJob.Trigger(recurringJob.Id);
            //}
        }


    }
}