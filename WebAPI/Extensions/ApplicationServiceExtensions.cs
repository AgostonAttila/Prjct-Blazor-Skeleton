using Application.Core;
using Application.Interfaces;
using Infrastructure.Email;
using Infrastructure.Security;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
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

		public static void UseErrorHandlingMiddleware(this IApplicationBuilder app)
		{
			app.UseMiddleware<ExceptionMiddleware>();
		}

		public static void UseHangfireDashboardAndJobs(this IApplicationBuilder app)
		{
			//app.UseHangfireDashboard();
			//RecurringJob.AddOrUpdate<Helper.HangfireJobs>(x => x.RefreshFundDatas(), "20 22 * * *");
		}

		public static void UseHealthChecks(this IApplicationBuilder app)
		{
			// app.UseHealthChecks("/health");
			app.UseHealthChecks("/health", new HealthCheckOptions
			{
				ResponseWriter = async (context, report) =>
				{
					context.Response.ContentType = "application/json";
					var response = new HealthCheckReponse
					{
						Status = report.Status.ToString(),
						HealthChecks = report.Entries.Select(x => new IndividualHealthCheckResponse
						{
							Component = x.Key,
							Status = x.Value.Status.ToString(),
							Description = x.Value.Description

						}),
						HealthCheckDuration = report.TotalDuration
					};
					await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
				}
			});
		}
	}
}
