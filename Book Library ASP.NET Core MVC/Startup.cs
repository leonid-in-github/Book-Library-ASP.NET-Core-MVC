using System;
using Book_Library_ASP.NET_Core_MVC.AppConfig;
using Book_Library_Repository_EF_Core.Contexts;
using Book_Library_Repository_EF_Core.Repositories;
using Book_Library_Repository_EF_Core.Servicies;
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
        private string _contentRootPath = "";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _contentRootPath = configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDistributedMemoryCache();

            RepositoryService.Register<BookLibraryRepository>(
                Configuration["ConnectionStrings:DefaultConnection"].ToString().Replace("%CONTENTROOTPATH%", _contentRootPath)
                );
            RepositoryService.SetSessionExpirationTimeInMinutes(20);

            services.AddSession(options =>
            {
                options.Cookie.Name = Configuration.GetSection("sessionConfig")["SessionCookieName"].ToString();
                options.IdleTimeout = TimeSpan.FromMinutes(RepositoryService.SESSIONEXPIRATIONTIMEINMINUTES);
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
                options.UseSqlServer(RepositoryService.ConnectionString<BookLibraryRepository>()));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //CookieAuthenticationOptions
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(RepositoryService.SESSIONEXPIRATIONTIMEINMINUTES);
                });

            services.AddControllersWithViews().AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
