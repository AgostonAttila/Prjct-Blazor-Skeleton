using Application.Core;
using Application.Interfaces;
using Infrastructure.Email;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Persistence;
using System.Collections.Generic;
using System.Configuration;

namespace WebAPI.Extensions
{
	public static class ApplicationServiceExtensions
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services,
			IConfiguration config)
		{
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
			});

			//belerakni többi db-t
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


			services.AddCors(opt =>
			{
				opt.AddPolicy("CorsPolicy", policy =>
				{
					policy
					.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowCredentials()
					.WithExposedHeaders("WWW-Authenticate", "Pagination")
					.WithOrigins(new string[] { "http://localhost:3000", "test" });
				});
			});
			//services.AddMediatR(typeof(List.Handler).Assembly);
			//services.AddAutoMapper(typeof(MappingProfiles).Assembly);
			services.AddScoped<IUserAccessor, UserAccessor>();
			//services.AddScoped<IPhotoAccessor, PhotoAccessor>();
			services.AddScoped<EmailSender>();
			services.AddSignalR();

			return services;
		}
	}
}
