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


			CreateMap<ApplicationRoles, RoleViewModel>()
				.ForMember(d => d.RoleName, s => s.MapFrom(p => p.Name))
				.ForMember(d => d.RoleName, s => s.MapFrom(p => p.NormalizedName))
				 .ForMember(d => d.RoleId, s => s.MapFrom(p => p.Id))
				.ReverseMap();

			CreateMap<ApplicationUsers, RegistertViewModel>()
			.ForMember(d => d.Email, s => s.MapFrom(p => p.Email))
			.ForMember(d => d.Email, s => s.MapFrom(p => p.UserName))
			.ForMember(d => d.MobileNo, s => s.MapFrom(p => p.PhoneNumber))
			.ReverseMap();

			CreateMap<ApplicationUsers, LoginViewModel>()
			.ForMember(d => d.Email, s => s.MapFrom(p => p.Email))
			.ReverseMap();
		}
	}
}
