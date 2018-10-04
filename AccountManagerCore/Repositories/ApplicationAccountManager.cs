using AccountCore.DataModels;
using AccountCore.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AccountCore.Repositories
{
	public class ApplicationAccountManager : IApplicationAccountManager
	{
		private readonly RoleManager<ApplicationRoles> roleManager;
		public ApplicationAccountManager(RoleManager<ApplicationRoles> _roleManager) {
			RoleManager<ApplicationRoles> roleManager = _roleManager;
		}

		public async Task<bool> AddApplicationUserAsync(string roleName)
		{
			if (await IsRoleApplicationRoles(roleName))
			{
				throw new InvalidProgramException();
			}
			throw new InvalidProgramException();
		}

		

		public async Task<bool> IsRoleApplicationRoles(string roleName)
		{
			ApplicationRoles applicationRoles = await roleManager.FindByNameAsync(roleName);
			return applicationRoles != null;
		}



	}
}
