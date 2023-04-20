using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
	public static class ServiceExtensions
	{	

		public static IServiceCollection AddPersistence (this IServiceCollection services,
	    IConfiguration config)
		{
			services.AddDbContext<DataContext>(options =>
			{
				options.UseSqlServer("Server=./;Database=skeleton;User Id=sa3;Password=Titkos!;TrustServerCertificate=True;");
			});

			//services.AddDbContext<DataContext>(options =>
			//{
			//	var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			//	if (env == "Development")
			//	{

			//		switch (config.GetConnectionString("DBType"))
			//		{
			//			case "mssql":
			//				options.UseSqlServer(config.GetConnectionString("sqlConnection"));
			//				break;
			//			case "sqlite":
			//				options.UseSqlite(config.GetConnectionString("sqliteConnection"));
			//				break;
			//			case "mysql":
			//				options.UseMySQL(config.GetConnectionString("mysqlConnection"));
			//				break;
			//			case "postgre":
			//				options.UseNpgsql(config.GetConnectionString("postgreConnection"));
			//				break;
			//			case "inmemory":
			//				options.UseInMemoryDatabase(config.GetConnectionString("inmemoryConnection"));
			//				break;
			//			default:
			//				options.UseSqlServer(config.GetConnectionString("sqlConnection"));
			//				break;
			//		}
			//	}
			//	else
			//	{
			//		options.UseSqlServer(config.GetConnectionString("sqlConnection"));
			//	}
			//});
			return services;
		}

			
	}
}
