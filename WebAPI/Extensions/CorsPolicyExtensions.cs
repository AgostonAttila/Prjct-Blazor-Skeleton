using Application.Interfaces;
using Infrastructure.Email;
using Infrastructure.Security;
using Microsoft.OpenApi.Models;

namespace WebAPI.Extensions
{
	public static class CorsPolicyExtensions
	{
		public static IServiceCollection AddCorsPolicy(this IServiceCollection services,
		  IConfiguration config)
		{
			services.AddCors(policy =>
			{
				policy.AddPolicy("CorsPolicy", opt => opt
				.AllowAnyOrigin()
				.AllowAnyHeader()
				.AllowAnyMethod()
				.WithExposedHeaders("X-Pagination"));
			});

			return services;
		}
	}
}
