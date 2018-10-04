using AccountCore.DataModels;
using Microsoft.EntityFrameworkCore;
using System;

namespace Science.City.EF.dbmodel
{
	public class dbcontext : DbContext
	{
		public DbSet<ApplicationUsers> ApplicationUsers { get; set; }

		public DbSet<ApplicationRoles> ApplicationRoles { get; set; }


		public dbcontext()
		{

			this.Database.EnsureCreated();
		}


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			modelBuilder.Entity<ApplicationUsers>(o =>
			{
				o.ToTable("Users")
				.HasKey(e => e.Id);
			});

			modelBuilder.Entity<ApplicationRoles>(o =>
			o.ToTable("AppRoles")
			.HasKey(e => e.Id)
			);

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
