using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Persistence.Configuration;
using System.Reflection.Emit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Persistence
{
	//dotnet ef migrations add InitialCreate -p Persistence/ -s WebAPI/
	//dotnet ef database update -p Persistence/ -s WebAPI/
	public class DataContext : IdentityDbContext<AppUser>
	{
		public DataContext(DbContextOptions options) : base(options)
		{
		}

        public DbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.ApplyConfiguration(new RoleConfiguration());												 
		}
		

		
	}
}

