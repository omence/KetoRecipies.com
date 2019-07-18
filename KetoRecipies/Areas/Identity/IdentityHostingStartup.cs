using System;
using KetoRecipies.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
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

                services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<KetoRecipiesContext>()
                    .AddDefaultTokenProviders();

                services.AddMvc()
        .AddRazorPagesOptions(options =>
        {
            options.AllowAreas = true;
            options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
            options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
            options.Conventions.AuthorizeAreaPage("Identity", "/Account/ForgotPassword");
            options.Conventions.AuthorizeAreaPage("Identity", "/Account/ResetPassword");
            options.Conventions.AuthorizeAreaPage("Identity", "/Account/ResetPasswordConfirmation");
        });

                services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = $"/Identity/Account/Login";
                    options.LogoutPath = $"/Identity/Account/Logout";
                    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
                });
                services.AddSingleton<IEmailSender, EmailSender>();

            });
        }
    }
}