using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Book_Library_ASP.NET_Core_MVC.Models.AppConfig;
using Book_Library_EF_Core_Proxy_Class_Library.Configuration;
using Book_Library_EF_Core_Proxy_Class_Library.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Book_Library_ASP.NET_Core_MVC
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
            services.AddControllersWithViews();

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.Cookie.Name = Configuration.GetSection("sessionConfig")["SessioCookieName"].ToString();
                options.IdleTimeout = TimeSpan.FromMinutes(Book_Library_EF_Core_Proxy_Class_Library.Constants.LibraryConstants.SESSIONEXPIRATIONTIMEINMINUTES);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<SessionConfig>(Configuration.GetSection("sessionConfig"));

            services.AddDbContext<BookLibraryContext>(options =>
        options.UseSqlServer(BookLibraryProxyConfiguration
                .GetInstanse().ConnectionString));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //CookieAuthenticationOptions
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(Book_Library_EF_Core_Proxy_Class_Library.Constants.LibraryConstants.SESSIONEXPIRATIONTIMEINMINUTES);
                });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            BookLibraryProxyConfiguration
                .GetInstanse()
                .SetupBookLibraryProxyConfiguration(Configuration["ConnectionStrings:DefaultConnection"].ToString());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
