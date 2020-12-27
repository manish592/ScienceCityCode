using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Science.City.Web.Areas.Membership.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Science.City.Web.ViewModels
{
    public partial class AccountViewModel
    {
        public RegiterViewModelWithRole RegistertViewModel { get; set; }
        public LoginViewModel LoginViewModel { get; set; }
    }

    public partial class LoginViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
    public class ApproveUser
    {
        [Display(Name = "Approved")]
        public bool IsApproved { get; set; }


        [Display(Name = "key")]
        public virtual string key { get; set; }

        public Guid KeyFK { get; set; }

        public Guid Id { get; set; }
    }

    public partial class RegistertViewModel : ApproveUser
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Name cannot be longer than 20 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Name cannot be longer than 20 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        //  [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        [RegularExpression(@"(?:\s+|)((0|(?:(\+|)91))(?:\s|-)*(?:(?:\d(?:\s|-)*\d{9})|(?:\d{2}(?:\s|-)*\d{8})|(?:\d{3}(?:\s|-)*\d{7}))|\d{10})(?:\s+|)", ErrorMessage = "Not a valid Mobile Number")]
        [Display(Name = "Mobile Number")]
        public string MobileNo { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Length should not less than{0}")]
        [MaxLength(10, ErrorMessage = "Invalid Password")]
        [Display(Name = "Password")]
        public string Password { get; set; }


        [Compare("Password", ErrorMessage = "Password not matched")]
        [MinLength(8, ErrorMessage = "Length should not less than{0}")]
        [MaxLength(10, ErrorMessage = "Invalid Password")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }


        [Display(Name = "Country")]
        [Range(1, int.MaxValue, ErrorMessage = "Select Country")]
        public int CountryFK { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select State")]
        [Display(Name = "State")]
        public int StateFK { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select City")]
        [Display(Name = "City")]
        public int CityFK { get; set; }


        public bool IsInternalRole = false;

        public bool IsLocked { get; set; }

        [Display(Name = "End")]
        public string End { get; set; }

		[Display(Name = "GST Number")]
		public string GSTNumber { get; set; }

		[Display(Name = "Name of the Press Reporter")]
		public string  PressReporterName { get; set; }

		[Display(Name = "News Paper Company")]
		public string NewsPaperCompany { get; set; }

		[Display(Name = "ID of the Press Card")]
		public string  PressIDCard { get; set; }
	}

    public class RegiterViewModelWithRole : RegistertViewModel
    {

        public Guid RoleId { get; set; }
        public IEnumerable<SelectListItem> GetRoles { get; set; }
        [Required]
        [Display(Name = "Register As")]
        public string RoleName { get; set; }
    }

    public partial class ForgetPasswordModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }

    public partial class ResetPasswordModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Length should not less than{0}")]
        [MaxLength(10, ErrorMessage = "Invalid MobileNo")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Password not matched")]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }

    public class MemberShipProfile : RegistertViewModel
    {
        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Pin Code")]
        [RegularExpression(@"^\d{6}?$", ErrorMessage = "Invalid Pin Code")]
        public string PinCode { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"(?:\s+|)((0|(?:(\+|)91))(?:\s|-)*(?:(?:\d(?:\s|-)*\d{9})|(?:\d{2}(?:\s|-)*\d{8})|(?:\d{3}(?:\s|-)*\d{7}))|\d{10})(?:\s+|)", ErrorMessage = "Not a valid Phone number")]
        public string PhoneNumber { get; set; }

        public string ImagePath { get; set; }

        public bool IsMembershipApproved { get; set; }

        public IFormFile Image { get; set; }
    }

    public class StaffMemberProfile : MemberShipProfile
    {
        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Select Designation")]
        [Range(1, int.MaxValue, ErrorMessage = "Select Designation")]
        public int DesignationFK { get; set; }

        [Display(Name = "Family Member Info")]
        public List<StaffMemberInfo> StaffMemberInfo { get; set; }

        public IEnumerable<string> SelectedCounts { get; set; }
    }

    public class TourOperatorProfile : MemberShipProfile
    {

        [Required]
        [Display(Name = "PAN No")]
        public string PanNo { get; set; }
        [Required]
        [Display(Name = "GST No")]
        public string GST { get; set; }
        [Display(Name = "Website")]
        [Url(ErrorMessage = "Please Enter a valid url")]
        public string Website { get; set; }
        [Required]
        [Display(Name = "Name of the Organization")]
        public string OrganizationName { get; set; }

        [Display(Name = "Tour Operator ID")]
        [Required(ErrorMessage = "Tour Operator ID is Required")]
        public override string key { get; set; }

        [Display(Name = "Contect Info")]
        public List<SchoolContectInfo> SchoolContectInfo { get; set; }
    }
    public class StaffMemberInfo
    {
        [Required]
        public string Name { get; set; }

        public string ImagePath { get; set; }

        public string ImageName { get; set; }

        [Required]
        public virtual int Relationship { get; set; }

        public IFormFile Image { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"(?:\s+|)((0|(?:(\+|)91))(?:\s|-)*(?:(?:\d(?:\s|-)*\d{9})|(?:\d{2}(?:\s|-)*\d{8})|(?:\d{3}(?:\s|-)*\d{7}))|\d{10})(?:\s+|)", ErrorMessage = "Not a valid Mobile Number")]
        public virtual string MobileNumber { get; set; }
    }

    public class SchoolContectInfo : StaffMemberInfo
    {

        public override int Relationship { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Mobile number")]
        public override string MobileNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

    }
}
