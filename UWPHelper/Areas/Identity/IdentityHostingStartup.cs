using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UWPHelper.Areas.Identity.Data;
using UWPHelper.Authorization;
using UWPHelper.Models;
using UWPHelper.Services.Implement;
using UWPHelper.Services.Interface;

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

                //UserManager注册
                services.AddDefaultIdentity<UWPHelperUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<IdentityContext>();

                //授权程序注册
                services.AddScoped<IAuthorizationHandler, SourceCodeAuthorizationHandler>();

                //Service注册
                services.AddSingleton<IDetectManager, DetectManager>();
                services.AddScoped<ISourceCodeManager,SourceCodeManager>();
            });
        }
    }
}