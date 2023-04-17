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

		
			services.Configure<AppSettings> (config.GetSection("AppSettings"));

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
