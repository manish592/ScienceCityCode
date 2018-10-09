using System;

namespace Science.City.Web.Models
{
	public class ErrorViewModel
	{
		public string RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
	
}