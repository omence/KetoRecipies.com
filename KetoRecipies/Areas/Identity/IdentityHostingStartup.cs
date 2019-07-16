using System;
using KetoRecipies.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(KetoRecipies.Areas.Identity.IdentityHostingStartup))]
namespace KetoRecipies.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<KetoRecipiesContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("KetoRecipiesContextConnection")));

                services.AddDefaultIdentity<ApplicationUser>()
                    .AddEntityFrameworkStores<KetoRecipiesContext>();
            });
        }
    }
}