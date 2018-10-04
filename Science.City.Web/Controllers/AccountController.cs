using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountCore.DataModels;
using AccountCore.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Science.City.Web.ViewModels;

namespace Science.City.Web.Controllers
{
    public class AccountController : Controller
    {

		private readonly IAccountManager accountManager;
		public AccountController(IAccountManager _accountManager)
		{
			accountManager = _accountManager;
		}

		public IActionResult Index()
        {
            return View();
        }

		public IActionResult List() {

			return View();
		}



		[HttpPost]
		public async Task<IActionResult> Index(RoleViewModel roleViewModel) {

			if (ModelState.IsValid)
			{
				var applicationRole = AutoMapper.Mapper.Map<ApplicationRoles>(roleViewModel);
				(bool result, string[] role) = await accountManager.CreateRoleAsync(applicationRole, null);

				if (result)
				{

				}
			}
			return View(roleViewModel);
		}



    }
}