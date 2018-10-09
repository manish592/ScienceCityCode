using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Science.City.Web.ViewModels
{

	public partial class AccountViewModel
	{

		public RegistertViewModel RegistertViewModel { get; set; }
		public LoginViewModel LoginViewModel { get; set; }
		
	}

	public partial class LoginViewModel {

		public string Email { get; set; }
		public string Password { get; set; }
		public bool RememberMe { get; set; }
	}
	public partial class RegistertViewModel
	{
		[Required]
		public string FirstName { get; set; }

		[Required]
		public string LastName { get; set; }

		[Required]
		public string Email { get; set; }

		[Required]
		[MaxLength(10,ErrorMessage ="Invalid MobileNo")]
		public string MobileNo { get; set; }

		[Required]
		[MinLength(8, ErrorMessage = "Length should not less than{0}")]
		[MaxLength(10, ErrorMessage = "Invalid MobileNo")]
		public string Password { get; set; }

		[Required]
		[Compare("Password", ErrorMessage = "Password not matched")]
		public string ConfirmPassword { get; set; }
	}


	
}
