using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountCore.DataModels;
using AccountCore.ServiceInjects;
using CRM.DatabaseServiceLayer.Services.Repository.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Science.City.EF.dbmodel;
using Science.City.EF.dbmodel.Models;
using StudentCounselling.Services.Repository.Services;

namespace Science.City.Web
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
			services.AddMvc();
			services.AddIdentity<ApplicationUsers, ApplicationRoles>()
				 .AddEntityFrameworkStores<dbcontext>()
				.AddDefaultTokenProviders();
			 services.AddDbContext<dbcontext>();
			services.Configure<IdentityOptions>(options =>
			{
				// Password settings
				options.Password.RequireDigit = true;
				options.Password.RequiredLength = 8;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequireLowercase = false;
				options.Password.RequiredUniqueChars = 1;

				// Lockout settings
				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
				options.Lockout.MaxFailedAccessAttempts = 100;
				options.Lockout.AllowedForNewUsers = true;

				// User settings
				options.User.RequireUniqueEmail = true;
				//


			});
			services.ConfigureApplicationCookie(options =>
			{
				// Cookie settings
				options.Cookie.HttpOnly = true;
				options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
				// If the LoginPath isn't set, ASP.NET Core defaults 
				// the path to /Account/Login.
				options.LoginPath = "/Account/LoginRegister";
				// If the AccessDeniedPath isn't set, ASP.NET Core defaults 
				// the path to /Account/AccessDenied.
				options.AccessDeniedPath = "/Account/AccessDenied";
				options.SlidingExpiration = true;
			});

			services.AddSession();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddTransient<IRepository<Packages>, Repository<Packages>>();

			
			services.AddAccountManager();
			AutoMapper.Mapper.Initialize(cfg => {

				cfg.AddProfile<Science.City.Web.Mapper.Mapper>();
			});
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
