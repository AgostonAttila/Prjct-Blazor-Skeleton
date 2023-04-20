using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Cors
{
	public static class ServiceExtension
	{
	
		public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
		{

			services.AddCors(opt =>
			{
				opt.AddPolicy("CorsPolicy", policy =>
				{
					policy
					.AllowAnyMethod()
					.AllowAnyHeader()
					.AllowCredentials()
					.WithExposedHeaders("WWW-Authenticate", "Pagination")
					.WithOrigins(new string[] { "http://localhost:7009" });
				});
			});

			return services;
		}

		public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app) =>
	     app.UseCors("CorsPolicy");
	}
}
