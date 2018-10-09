using AccountCore.DataModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Science.City.EF.dbmodel.Models;
using System;

namespace Science.City.EF.dbmodel
{

	public enum EnumPackageType:int{

	}

	public class dbcontext : DbContext   // IdentityDbContext<ApplicationUsers,ApplicationRoles,Guid>
	{
		public DbSet<ApplicationUserDetail> UserDetails { get; set; }
		public DbSet<ApplicationUsers> Users { get; set; }
		public DbSet<ApplicationRoles> Roles { get; set; }
		public DbSet<Packages> Packages { get; set; }
		public DbSet<Ticket> Tickets { get; set; }
		public DbSet<UserAccessPermission> UserAccessPermissions { get; set; }


		public dbcontext()
		{

			this.Database.EnsureCreated();
		}


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			
			modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles").HasKey(p => new { p.UserId, p.RoleId });
			modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaim");
			modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogin").HasKey(p => new { p.UserId });
			modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserToken").HasKey(p => p.UserId);
			modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("UserRoleClaim").HasKey(p => p.RoleId);
			modelBuilder.Entity<ApplicationUsers>().ToTable("Users");
			modelBuilder.Entity<ApplicationRoles>().ToTable("AppRoles");
			modelBuilder.Entity<Packages>().ToTable("Packages").HasKey(p => p.PackageId);
			modelBuilder.Entity<Ticket>().ToTable("Tickets").HasKey(p => p.Id);
			modelBuilder.Entity<UserAccessPermission>().ToTable("UserAccessPermission").HasKey(p => p.Id);

			base.OnModelCreating(modelBuilder);
		}


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				
				optionsBuilder.UseSqlServer("Data Source=192.168.1.249\\sqlexpress;Initial Catalog=ScienceCityWeb;User ID=sa;Password=slinfy@123");
			}
		}
	}
}
