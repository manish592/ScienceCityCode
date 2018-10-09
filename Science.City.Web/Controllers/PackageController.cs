using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRM.DatabaseServiceLayer.Services.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Science.City.EF.dbmodel.Models;
using Science.City.Web.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Science.City.Web.Controllers
{
	public class PackageController : Controller
	{
		private readonly IRepository<Packages> repository;
		public PackageController( IRepository<Packages> _repository) {
			repository = _repository;
		}

		// GET: /<controller>/
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Create()
		{
			var p=repository.GetAll().ToList();
			return View();
		}

		[HttpPost]
		public IActionResult Create(PackageViewModel packageViewModel)
		{
			return View();
		}
	}
}
