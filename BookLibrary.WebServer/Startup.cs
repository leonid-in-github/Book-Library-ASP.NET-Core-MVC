using BookLibrary.Storage;
using BookLibrary.Storage.Contexts;
using BookLibrary.Storage.Repositories;
using BookLibrary.WebServer.AppConfig;
using BookLibrary.WebServer.Models.JQueryModels;
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
    public class Startup(IConfiguration configuration)
    {
        private readonly string _contentRootPath = configuration.GetValue<string>(WebHostDefaults.ContentRootKey);

        public IConfiguration Configuration { get; } = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IBooksRepository, BooksRepository>();

            services.AddControllers(options =>
            {
                options.ModelBinderProviders.Insert(0, new DataTableParametersBinderProvider());
            });

            services.AddControllersWithViews();

            services.AddDistributedMemoryCache();

            StorageParameters.ConnectionString =
                Configuration["ConnectionStrings:DefaultConnection"].ToString().Replace("%CONTENTROOTPATH%", _contentRootPath);
            StorageParameters.SessionTimeoutInMinutes = int.Parse(Configuration["sessionConfig:SessionTimeoutInMinutes"]);

            services.AddSession(options =>
            {
                options.Cookie.Name = Configuration.GetSection("sessionConfig")["SessionCookieName"].ToString();
                options.IdleTimeout = TimeSpan.FromMinutes(StorageParameters.SessionTimeoutInMinutes);
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
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(StorageParameters.SessionTimeoutInMinutes);
                    options.LogoutPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
                    options.Cookie.Name = Configuration["sessionConfig:SessionCookieName"].ToString();
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
