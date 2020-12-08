using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using LeaveManagment.Repository;
using LeaveManagment.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LeaveManagment.Contract;
using AutoMapper;
using LeaveManagment.Mappings;
using Microsoft.AspNetCore.Http;

namespace LeaveManagment
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            // add this file name to your .gitignore file
            // so you can create it and use on your local dev machine
            // remember last config source added wins if it has the same settings
            builder.AddJsonFile("appsettings.Development.json", optional: true);
            builder.AddEnvironmentVariables();

            Configuration = builder.Build();
            _env = env;
        }
        public IWebHostEnvironment _env { get; set; }
        public IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (_env.IsDevelopment())
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                           options.UseSqlServer(
                               Configuration.GetConnectionString("DefaultConnection")));
            }
            else if (_env.IsProduction())
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                          options.UseSqlServer(
                              Configuration.GetConnectionString("AzureConnection")));
            }
          
           

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddAutoMapper(typeof(Maps));

            services.AddDefaultIdentity<Employee>().AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            //this need to be added for chrom cookie issue
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Unspecified;
            });

            services.AddAntiforgery(opts => {
                opts.Cookie.SameSite = SameSiteMode.Unspecified;
            });

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
                              IWebHostEnvironment env,
                              UserManager<Employee> userManager,
                              RoleManager<IdentityRole> roleManager)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

           ManageRoles.CreateDefault(userManager,roleManager);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
