using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Listrr.Areas.Identity.IdentityHostingStartup))]
namespace Listrr.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}