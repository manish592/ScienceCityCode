using System;
using System.Collections.Generic;
using System.Text;

namespace Science.City.EF.dbmodel.Models
{
	public class Ticket
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool isPakcageTicket { get; set; }
	}
}
