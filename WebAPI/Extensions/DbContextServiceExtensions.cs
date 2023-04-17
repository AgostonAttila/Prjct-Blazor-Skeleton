using Application.Interfaces;
using Infrastructure.Email;
using Infrastructure.Security;
using Microsoft.OpenApi.Models;

namespace WebAPI.Extensions
{
	public static class DbContextServiceExtensions
	{
		public static IServiceCollection AddDbContextServiceExtensions(this IServiceCollection services,
		IConfiguration config)
		{
			
			services.AddDbContext<DataContext>(options =>
			{
				var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
				if (env == "Development")
				{

					switch (config.GetConnectionString("DBType"))
					{
						case "mssql":
							options.UseSqlServer(config.GetConnectionString("sqlConnection"));
							break;
						case "sqlite":
							options.UseSqlite(config.GetConnectionString("sqliteConnection"));
							break;
						case "mysql":
							options.UseMySQL(config.GetConnectionString("mysqlConnection"));
							break;
						case "postgre":
							options.UseNpgsql(config.GetConnectionString("postgreConnection"));
							break;
						case "inmemory":
							options.UseInMemoryDatabase(config.GetConnectionString("inmemoryConnection"));
							break;
						default:
							options.UseSqlServer(config.GetConnectionString("sqlConnection"));
							break;
					}
				}
				else
				{
					options.UseSqlServer(config.GetConnectionString("sqlConnection"));
				}
			});



			return services;
		}
	}
}
