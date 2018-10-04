using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Science.City.Web.ViewModels
{
	public class RoleViewModel
	{

		public RoleViewModel()
		{

			this.RoleId =Guid.NewGuid();
		}

		public Guid RoleId { get; set; }


		[Required]
		public string RoleName { get; set; }
	}
}
