[assembly: HostingStartup(typeof(CleanArchi.Web.Areas.Identity.IdentityHostingStartup))]
namespace CleanArchi.Web.Areas.Identity;

public class IdentityHostingStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
        });
    }
}
