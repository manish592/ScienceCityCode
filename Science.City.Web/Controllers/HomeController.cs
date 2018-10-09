using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AccountCore.DataModels;
using AccountCore.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Science.City.Web.Helpers;
using Science.City.Web.Models;
using Science.City.Web.ViewModels;

namespace Science.City.Web.Controllers
{
	public class HomeController : Controller
	{
		private readonly IAccountManager accountManager;
		public HomeController(IAccountManager _accountManager)
		{

			accountManager = _accountManager;
		}
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> login(AccountViewModel viewModel)
		{

			if (ModelState.IsValid)
			{
				ApplicationUsers applicationUser = AutoMapper.Mapper.Map<ApplicationUsers>(viewModel.LoginViewModel);
				(bool sucess, string[] result, ApplicationUsers _ApplicationUsers) = await accountManager.CheckPasswordSignInAsync(viewModel.LoginViewModel.Email, viewModel.LoginViewModel.Password, viewModel.LoginViewModel.RememberMe);
				if (sucess)
				{
					await accountManager.SignInAsync(_ApplicationUsers, true);
					return RedirectToActionPermanent("Index", "account");
				}

				ModelState.AddModelError(EnumAlert.Error.ToString(), string.Join(",", result));
			}

			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> singup(AccountViewModel viewModel)
		{

			if (ModelState.IsValid)
			{
				ApplicationUsers applicationUser = AutoMapper.Mapper.Map<ApplicationUsers>(viewModel.RegistertViewModel);
				ApplicationRoles roleName = await accountManager.GetRoleByNameAsync(EnumApplicationUser.USER.ToString());
				(bool success, string[] result) = await accountManager.CreateUserAsync(applicationUser, new string[] { roleName.Name }, viewModel.RegistertViewModel.Password);
				if (success)
				{
				}
				else
				{
					ModelState.AddModelError(EnumAlert.Error.ToString(), string.Join(",", result));
				}
			}
			return RedirectToAction("index");
		}
	}
}
