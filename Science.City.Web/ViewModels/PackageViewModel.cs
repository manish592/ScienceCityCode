using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Science.City.Web.ViewModels
{
	public class PackageViewModel
	{
		public Guid PackageId { get; set; }

		[Required]
		[Display(Name = "Package Name")]
		public string PackageName { get; set; }

		[Required]

		public int PackageType { get; set; }

		[Required]
		public int Classification { get; set; }   //Frequent and Non Frequent.

		[Display(Name = "Shows In Pack")]
		public bool ShowsInPack { get; set; } = false;

		[Required]
		[Display(Name = "Rate of Package")]
		public decimal RateofPackage { get; set; }

		[Display(Name = "Complimentary Applicable")]
		[Required]
		public bool CpTckAp { get; set; } = false;
	}
}
