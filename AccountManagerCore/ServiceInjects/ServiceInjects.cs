using AccountCore.Repositories;
using AccountCore.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AccountCore.ServiceInjects
{
  public static  class ServiceInjects
    {
		public static IServiceCollection AddAccountManager(this IServiceCollection services)
		{
			services.AddTransient<IAccountManager, AccountManager>();
			return services;
		}


		public static IApplicationBuilder AddSeedUsers(this IApplicationBuilder app) {

			using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				var accountManager = scope.ServiceProvider.GetService<IAccountManager>();
			//	BaseAccountManager baseAccountManager = new BaseAccountManager(accountManager)
			}
			return app;

		}

		//public static void UseSqlTableDependency<T>(this IApplicationBuilder services, string connectionString)
		//{
		//	var serviceProvider = services.ApplicationServices;
		//	var subscription = (T)serviceProvider.GetService(typeof(T));
		//	subscription.Configure(connectionString);
		//}
	}
}
