using System;
using Application.Core;
using AutoMapper;
using Infrastructure.Email;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Persistence;

namespace API.Extensions
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

		
			services.AddDbContext<DbContext>(options =>
			{
				var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

				//string connStr = (env == "Development") ? config.GetConnectionString("DefaultConnection") : " ";

				switch (config.GetConnectionString("DBType"))
				{
					case "mssql":
						options.UseSqlServer(config.GetConnectionString("ConnectionMSSQL"));
						break;
					case "sqlite":
						options.UseSqlite(config.GetConnectionString("ConnectionSQLite"));
						break;
					case "mysql":
						options.UseMySQL(config.GetConnectionString("ConnectionMySQL"));
						break;
					case "postgre":
						options.UseNpgsql(config.GetConnectionString("ConnectionPostgre"));
						break;
					case "inmemory":
						options.UseInMemoryDatabase(config.GetConnectionString("InMemoryDB"));
						break;
					default:
						options.UseSqlServer(config.GetConnectionString("ConnectionMSSQL"));
						break;
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
			services.AddScoped<EmailSender>();
			services.AddSignalR();

			return services;
		}
	}
}
