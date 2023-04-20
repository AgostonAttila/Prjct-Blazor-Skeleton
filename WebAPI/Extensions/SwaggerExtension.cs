using Infrastructure.Middlawares;
using Microsoft.OpenApi.Models;

namespace WebAPI.Extensions
{
	public static class SwaggerExtension
	{
		public static IServiceCollection AddSwaggerExtension(this IServiceCollection services)
		{
			//services.AddSwaggerGen(c =>
			//{
			//	c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
			//});

			services.AddSwaggerGen(c =>
			{
				//c.IncludeXmlComments(string.Format(@"{0}\Skeleton.WebApi.xml", System.AppDomain.CurrentDomain.BaseDirectory));
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "Skeleton.WebApi",
					Description = "This Api will be responsible for overall data distribution and authorization.",
					Contact = new OpenApiContact
					{
						Name = "aa",
						Email = "xxx",
						Url = new Uri("https://google.com"),
					}
				});
				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					Description = "Input your Bearer token in this format - Bearer {your token here} to access this API",
				});
				c.AddSecurityRequirement(new OpenApiSecurityRequirement
						{
							{
								new OpenApiSecurityScheme
								{
									Reference = new OpenApiReference
									{
										Type = ReferenceType.SecurityScheme,
										Id = "Bearer",
									},
									Scheme = "Bearer",
									Name = "Bearer",
									In = ParameterLocation.Header,
								}, new List<string>()
							},
						});
			});
			return services;
		}

		internal static IApplicationBuilder UseUseSwaggerExt(this IApplicationBuilder app)
		{
			app.UseSwagger();
			app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));

			return app;
		}

		

	}
}
