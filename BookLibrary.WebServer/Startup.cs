using BookLibrary.Storage;
using BookLibrary.Storage.Contexts;
using BookLibrary.Storage.Repositories;
using BookLibrary.WebServer.AppConfig;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace BookLibrary.WebServer
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
            services.AddScoped<IBookLibraryRepository, BookLibraryRepository>();
            services.AddScoped<AccountRepository, AccountRepository>();
            services.AddScoped<SessionRepository, SessionRepository>();
            services.AddScoped<BooksRepository, BooksRepository>();

            services.AddControllersWithViews();

            services.AddDistributedMemoryCache();

            StorageParameters.ConnectionString =
                Configuration["ConnectionStrings:DefaultConnection"].ToString().Replace("%CONTENTROOTPATH%", _contentRootPath);
            StorageParameters.SESSIONEXPIRATIONTIMEINMINUTES = 20;

            services.AddSession(options =>
            {
                options.Cookie.Name = Configuration.GetSection("sessionConfig")["SessionCookieName"].ToString();
                options.IdleTimeout = TimeSpan.FromMinutes(StorageParameters.SESSIONEXPIRATIONTIMEINMINUTES);
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
                options.UseSqlServer(StorageParameters.ConnectionString));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => //CookieAuthenticationOptions
                {
                    options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(StorageParameters.SESSIONEXPIRATIONTIMEINMINUTES);
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
