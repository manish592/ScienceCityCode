using AccountCore.Repositories.Interfaces;
using EF.dbmodel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Science.City.EF.dbmodel.Models;
using Science.City.QueryModel.Common;
using Science.City.Web.Areas.Admin.Controllers;
using Science.City.Web.Helpers;
using Science.City.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitsOfWork.Service.Interface;
using static Science.City.EF.Service.Helpers.Common;

namespace Science.City.Web.Areas.Admin.Controllers
{
	public class UsersController : BaseController
	{
		private readonly IAccountManager accountManager;
		private readonly IAccountGenerateToken email;
		private readonly IUnitOfWork repository;
		private readonly ILogger<UsersController> logger;
		private readonly IMemoryCache memoryCache;

		public UsersController(IAccountManager _accountManager, IAccountGenerateToken _email, IUnitOfWork _repository, ILogger<UsersController> _logger, IMemoryCache _memoryCache)
		{
			accountManager = _accountManager;
			email = _email;
			repository = _repository;
			logger = _logger;
			memoryCache = _memoryCache;
		}

		// GET: Users
		public IActionResult Index(string id)
		{
			var routeValues = Convert.ToString(Url.ActionContext.RouteData.Values["id"]);
			bool includeInternal = (routeValues == "Admin");

			IQueryable<ApplicationUsers> AppUsers = repository.applicationUserService.GetQuery().AsNoTracking();
			IQueryable<ApplicationRoles> AppRoles = repository.applicationRolesService.GetQuery().AsNoTracking();
			IQueryable<IdentityUserRole<Guid>> UserRole = repository.userRolesService.GetQuery().AsNoTracking();
			// list = UserRole.Where(d => d.UserId == new Guid("ea5fe95b-74c2-4b4a-8a1f-54a7deebdf7a")).ToList();
			var query = (from u in AppUsers
						 join ur in UserRole on u.Id equals ur.UserId
						 join ar in AppRoles on ur.RoleId equals ar.Id
						 where
					   ur.RoleId == new Guid(id)
						 //includeInternal ?
						 //u.IsInternalRole == true
						 //: (u.IsInternalRole == null || u.IsInternalRole == false)

						 select new RegiterViewModelWithRole
						 {
							 Email = u.Email,
							 FirstName = u.FirstName + " " + u.LastName,
							 MobileNo = u.PhoneNumber,
							 Id = u.Id,
							 RoleId = ur.RoleId,
							 RoleName = ar.Name,
							 IsInternalRole = u.IsInternalRole ?? false,
							 IsLocked = u.LockoutEnabled,
							 GSTNumber = u.GSTNumber
						 }


						 ).AsQueryable();



			//if (!includeInternal)
			//{
			//    var _result = query.Where(q => q.RoleName == routeValues).ToList();
			//    return View(_result);
			//}
			return View(query);
		}

		// GET: Users/Details/5
		public async Task<ActionResult> Detail(Guid id)
		{
			if (id != Guid.Empty)
			{
				ApplicationUsers applicationUser = await accountManager.GetUserByIdAsync(id.ToString());
				if (applicationUser != null)
				{
					var RoleId = repository.applicationRolesService.GetUserRoleId(applicationUser.Id);
					if (RoleId == Guid.Empty)
					{
						ModelState.AddModelError(Helpers.EnumAlert.Error.ToString(), "USER don't have any role");
					}
					RegiterViewModelWithRole registertViewModel = AutoMapper.Mapper.Map<RegiterViewModelWithRole>(applicationUser);
					registertViewModel.RoleId = RoleId;
					registertViewModel.IsLocked = applicationUser.LockoutEnabled;
					registertViewModel.RoleName = new RoleHelper(repository).SelectedRoleName(RoleId);
					registertViewModel.GSTNumber = applicationUser.GSTNumber;
					return View(registertViewModel);
				}
			}
			return View(new RegiterViewModelWithRole());
		}

		// GET: Users/Create
		public ActionResult Create()
		{
			var registertViewModel = new RegiterViewModelWithRole();
			registertViewModel.GetRoles = new RoleHelper(repository).GetRoles(Guid.Empty);
			return View(registertViewModel);
		}

		// POST: Users/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(RegiterViewModelWithRole registertViewModel)
		{
			try
			{
				ModelState.Remove("RoleName");
				if (ModelState.IsValid)
				{
					ApplicationUsers applicationUsers = AutoMapper.Mapper.Map<ApplicationUsers>(registertViewModel);
					applicationUsers.Id = Guid.NewGuid();
					applicationUsers.IsInternalRole = true;
					ApplicationRoles roleName = await accountManager.GetRoleByIdAsync(registertViewModel.RoleId.ToString());
					(bool result, string[] response) = await accountManager.CreateUserAsync(applicationUsers, new string[] { roleName.Name }, registertViewModel.Password);
					if (result)
					{
						logger.LogInformation($"Successfully Created ${registertViewModel.Email}");
						ModelState.AddModelError(EnumAlert.Info.ToString(), "Successfully Created");
						return RedirectToAction(nameof(Index), new { Id = registertViewModel.RoleId });
					}
					else
					{
						logger.LogInformation($"ModelState No Valied ${registertViewModel.Email}${response.JoinError()}");
						ModelState.AddModelError(EnumAlert.Info.ToString(), response.JoinError());
						return View(registertViewModel);
					}
				}

			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error Occured ${registertViewModel.Email}");
				registertViewModel.GetRoles = new RoleHelper(repository).GetRoles(Guid.Empty);
				return View(registertViewModel);
			}
			registertViewModel.GetRoles = new RoleHelper(repository).GetRoles(Guid.Empty);
			return View(registertViewModel);
		}

		// GET: Users/Edit/5
		public async Task<ActionResult> Edit(Guid id)
		{
			if (id != Guid.Empty)
			{
				ApplicationUsers applicationUsers = await accountManager.GetUserByIdAsync(id.ToString());
				if (applicationUsers != null)
				{
					var RoleId = repository.applicationRolesService.GetUserRoleId(applicationUsers.Id);
					RegiterViewModelWithRole registertViewModel = AutoMapper.Mapper.Map<RegiterViewModelWithRole>(applicationUsers);
					registertViewModel.RoleId = RoleId;
					registertViewModel.IsLocked = applicationUsers.LockoutEnabled;
					registertViewModel.GSTNumber = applicationUsers.GSTNumber;
					registertViewModel.NewsPaperCompany = applicationUsers.NewsPaperCompany;
					registertViewModel.PressReporterName = applicationUsers.PressReporterName;
					registertViewModel.PressIDCard = applicationUsers.PressIDCard;
					registertViewModel.GetRoles = new RoleHelper(repository).GetRoles(RoleId);
					return View(registertViewModel);
				}
			}
			return NotFound();
		}
		// POST: Users/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit(RegiterViewModelWithRole registertViewModel)
		{
			try
			{
				if (registertViewModel != null)
				{
					Task<ApplicationUsers> applicationUsers = accountManager.GetUserByIdAsync(registertViewModel.Id.ToString());
					Task<ApplicationRoles> roleName = accountManager.GetRoleByIdAsync(registertViewModel.RoleId.ToString());
					ApplicationUsers _applicationUsers = await applicationUsers;
					ApplicationRoles _roleName = await roleName;
					if (_applicationUsers != null && _roleName != null)
					{
						_applicationUsers.Email = registertViewModel.Email;
						_applicationUsers.MobileNumber = registertViewModel.MobileNo;
						_applicationUsers.LockoutEnabled = registertViewModel.IsLocked;
						_applicationUsers.GSTNumber = registertViewModel.GSTNumber;
						_applicationUsers.CountryFK = registertViewModel.CountryFK;
						_applicationUsers.StateFK = registertViewModel.StateFK;
						_applicationUsers.CityFK = registertViewModel.CityFK;
						_applicationUsers.NewsPaperCompany = registertViewModel.NewsPaperCompany;
						_applicationUsers.PressReporterName = registertViewModel.PressReporterName;
						_applicationUsers.PressIDCard = registertViewModel.PressIDCard;
						(bool status, string[] response) = await accountManager.UpdateUserAsync(_applicationUsers, new string[] { _roleName.Name });
						if (status)
						{
							return RedirectToAction("Index", "Users", new { @id = registertViewModel.RoleId });
						}
						else
						{
							ModelState.AddModelError(Helpers.EnumAlert.Error.ToString(), string.Join(",", response));
						}
					}
					else
					{
						ModelState.AddModelError(Helpers.EnumAlert.Error.ToString(), "User or Role not found");
					}
				}
			}
			catch (Exception ex)
			{
				ModelState.AddModelError(Helpers.EnumAlert.Error.ToString(), string.Join(",", ex.InnerException));
				return View(registertViewModel);
			}
			registertViewModel.GetRoles = new RoleHelper(repository).GetRoles(Guid.Empty);
			return View(registertViewModel);
		}

		// GET: Users/Delete/5
		public ActionResult Delete(int id)
		{
			return View();
		}

		public IActionResult Settings(Guid id, Guid RoleId)
		{
			if (id != Guid.Empty)
			{
				var _userSettings = new UserMappingSettings();
				_userSettings.UserId = id;
				_userSettings.RoleId = RoleId;
				var _userPermission = repository.userAccessPermissionService.
									  GetFirstOrDefault(u => u.UserId == id);



				var appPermissions = GetUserConfig();
				if (_userPermission != null)
				{

					var userconfg = JsonConvert.DeserializeObject<List<UserConfig>>(_userPermission.UserConfig);

					var permission = (from ap in appPermissions
									  join uc in userconfg
									  on ap.Id equals uc.Id into upap
									  from p in upap.DefaultIfEmpty()
									  select new PermissionsViewModel
									  {
										  Id = ap.Id,   //GUID
										  Selected = p == null ? false : p.Value,
										  FriendlyName = ap.FriendlyName,
										  isChecked = p == null ? "" : p.Value ? "on" : "",
										  Name = ap.Name,
										  Detail = ap.Detail,
										  Type = ap.Type
									  }
									  ).ToList();
					_userSettings.Permissions = permission.OrderByDescending(d => d.Type);
					_userSettings.Reports = repository.Report.GetReports(id);
					return View(_userSettings);
				}
				_userSettings.Permissions = appPermissions;
				//_userSettings.Reports = repository.Report.GetAll().Select(d => new SelectListItem
				//{
				//    Text = d.Name,
				//    Value = d.Id.ToString(),
				//    Key = d.Key.ToString(),
				//    Header = d.Header
				//}).OrderBy(d => d.Data).ToList();
				_userSettings.Reports = repository.Report.GetReports(id);
				return View(_userSettings);
			}
			return NotFound();
		}

		[HttpPost]
		[DisableRequestSizeLimit]
		public IActionResult Settings(UserMappingSettings _userSettings)
		{
			if (_userSettings.Permissions != null && _userSettings.Permissions.Any())
			{
				for (var i = 0; i < _userSettings.Permissions.Count(); i++)
				{
					ModelState.Remove($"Permissions[{i}].Name");
					ModelState.Remove($"Permissions[{i}].FriendlyName");
					ModelState.Remove($"Permissions[{i}].Detail");
				}
				if (ModelState.IsValid)
				{
					var _userConfig = JsonConvert.SerializeObject(
						_userSettings.Permissions.Select(p => new UserConfig
						{
							Id = p.Id,
							Value = string.IsNullOrEmpty(p.isChecked) && p.isChecked != "on" ? false : true,
							Type = p.Type
						})
						);

					var _userAccess = repository.userAccessPermissionService.GetFirstOrDefault(uc => uc.UserId == _userSettings.UserId);
					if (_userAccess != null)
					{
						_userAccess.UserConfig = _userConfig;
						repository.userAccessPermissionService.Update(_userAccess);
					}
					else
					{
						repository.userAccessPermissionService.Add(new UserAccessPermission
						{
							Id = Guid.NewGuid(),
							UserId = _userSettings.UserId,
							UserConfig = _userConfig
						});
					}
					repository.SaveChanges();
					return RedirectToAction(nameof(Settings), new { id = _userSettings.UserId });
				}
			}
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id, IFormCollection collection)
		{
			try
			{
				// TODO: Add delete logic here
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}
		private IEnumerable<PermissionsViewModel> GetUserConfig()
		{
			var settings = repository.applicationPermissionsService
							.GetAll()
							.Select(s => new PermissionsViewModel
							{
								Detail = s.Detail,
								FriendlyName = s.FriendlyName,
								Name = s.Name,
								Id = s.Id,
								Type = UserConfigType.Setting.ToString()
							}).ToList();
			var items = repository.Item
							.GetAll()
							.Where(d => d.IsActive)
							.Select(s => new PermissionsViewModel
							{
								Name = s.Name,
								Id = s.Id,
								Type = UserConfigType.Item.ToString()
							}).ToList();
			settings.AddRange(items);
			var packages = repository.Packages
						   .GetAll()
						   .Where(d => d.IsActive)
						   .Select(s => new PermissionsViewModel
						   {
							   Name = s.PackageName,
							   Id = s.Id,
							   Type = UserConfigType.Package.ToString()
						   }).ToList();
			settings.AddRange(packages);
			return settings.OrderByDescending(d => d.Type);
		}

		public async Task<IActionResult> ReportPermission(List<Guid> ReportFK, Guid UserId)
		{
			List<UserReportLink> list = new List<UserReportLink>();
			foreach (var item in ReportFK)
			{
				list.Add(new UserReportLink { ReportFK = item, UserFK = UserId });
			}
			List<UserReportLink> existingPermission = repository.UserReport.Find(d => d.UserFK == UserId).ToList();
			if (existingPermission != null)
				repository.UserReport.RemoveRange(existingPermission);

			repository.UserReport.AddRange(list);
			await repository.SaveChanges();

			memoryCache.Remove("ReportMenu");

			return RedirectToAction(nameof(Settings), new { id = UserId });
		}
	}
}