using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountCore.DataModels;
using AutoMapper;
using Science.City.Web.ViewModels;

namespace Science.City.Web.Mapper
{
    public class Mapper : Profile
	{
		public Mapper()
		{
			CreateMap<ApplicationRoles, RoleViewModel >()
				.ForMember(d => d.RoleName, s => s.MapFrom(p => p.Name))
				.ForMember(d => d.RoleName, s => s.MapFrom(p => p.NormalizedName))
				 .ForMember(d => d.RoleId, s => s.MapFrom(p => p.Id))
				.ReverseMap();
		}
	}
}
