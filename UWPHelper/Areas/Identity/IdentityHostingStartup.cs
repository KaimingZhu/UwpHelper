using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UWPHelper.Areas.Identity.Data;
using UWPHelper.Models;

[assembly: HostingStartup(typeof(UWPHelper.Areas.Identity.IdentityHostingStartup))]
namespace UWPHelper.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<IdentityContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("IdentityContextConnection")));

                services.AddDefaultIdentity<UWPHelperUser>()
                    .AddEntityFrameworkStores<IdentityContext>();
            });
        }
    }
}