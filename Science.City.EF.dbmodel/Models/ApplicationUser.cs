using System;
using DevExpress.Xpo;

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace EF.dbmodel.Models
{

    public class ApplicationUsers : IdentityUser<Guid>
    {
        public ApplicationUsers() { }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string AlternateNumber { get; set; }
        public bool? IsInternalRole { get; set; } = false;

        public int CountryFK { get; set; }
        public int StateFK { get; set; }
        public int CityFK { get; set; }
        public bool IsApproved { get; set; } = true;
        public Guid ApprovedBy { get; set; }
        public string ApprovedKey { get; set; }
        public string Address { get; set; }
        public string PinCode { get; set; }
        public string ImagePath { get; set; }
        public bool IsProfileComplete { get; set; } = false;

        public ICollection<MemberShipVisit> MemberShipVisits { get; set; }

        public UserDetail UserDetail { get; set; }
        public bool IsSync { get; set; } = false;
		public string GSTNumber { get; set; }
		public bool Loginstatus { get; set; }
		public string PressReporterName { get; set; }
		public string NewsPaperCompany { get; set; }
		public string PressIDCard { get; set; }
	}

    public partial class ApplicationRoles : IdentityRole<Guid>
    {
        public ApplicationRoles() { }
        public ApplicationRoles(string roleName) : base(roleName) { }
        public bool IsInternalRole { get; set; } = false;
        public string AliasName { get; set; }
        public bool IsSync { get; set; } = false;



    }
    public partial class ApplicationUserRole : IdentityUserRole<Guid>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ApplicationUserRoleId { get; set; }
        public bool IsSync { get; set; } = false;
    }
}