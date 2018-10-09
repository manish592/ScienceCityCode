using System;
using System.Collections.Generic;
using System.Text;

namespace Science.City.EF.dbmodel.Models
{
	public class Packages
	{
		public Guid PackageId { get; set; }
		public string PackageName { get; set; }
		public int PackageType { get; set; }
		public int Classification { get; set; }   //Frequent and Non Frequent.
		public bool ShowsInPack { get; set; }
		public decimal RateofPackage { get; set; }
		public bool CpTckAp { get; set; }  //•	Complimentary tickets applicable or not applicable for packs.
	}



}
