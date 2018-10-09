using AccountCore.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Science.City.EF.dbmodel.Models
{
	public class UserAccessPermission
	{
		
		public Guid Id { get; set; }

		[ForeignKey("applicationUsers")]
		public Guid UserId { get; set; }
		public ApplicationUsers applicationUsers { get; set; }

		public string UserConfig { get; set; }
	}
}
