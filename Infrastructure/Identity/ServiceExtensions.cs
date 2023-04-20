using System.Text;
using Domain;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace WebAPI.Extensions
{
	public static class ServiceExtensions
	{
		public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
		{
			services.AddIdentityCore<AppUser>(opt =>
			{
				opt.Lockout.AllowedForNewUsers = true;
				opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
				opt.Lockout.MaxFailedAccessAttempts = 3;
				opt.Password.RequireNonAlphanumeric = true;
				opt.Password.RequiredUniqueChars = 1;
				opt.Password.RequireDigit = true;
				opt.Password.RequiredLength = 8;
				opt.Password.RequireUppercase = true;
				opt.Password.RequireUppercase = true;
				opt.SignIn.RequireConfirmedEmail = false;
				opt.User.RequireUniqueEmail = true;			
			})
			.AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<DataContext>()
			.AddSignInManager<SignInManager<AppUser>>()
			.AddDefaultTokenProviders();

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("JwtSettings").GetSection("securityKey").Value));

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(opt =>
			{
				opt.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = key,
					ValidateIssuer = false,
					ValidateAudience = false,
					ValidateLifetime = true,
					ClockSkew = TimeSpan.Zero				
				};
				opt.Events = new JwtBearerEvents
				{
					OnMessageReceived = context =>
					{
						var accesToken = context.Request.Query["acces_token"];
						var path = context.HttpContext.Request.Path;
						if (!String.IsNullOrWhiteSpace(accesToken) && (path.StartsWithSegments("/chat")))
						{

							context.Token = accesToken;
						}
						return Task.CompletedTask;
					}
				};


			});

			services.AddAuthorization(opt =>
			{
				opt.AddPolicy("IsActivityHost", policy =>
				{
					policy.Requirements.Add(new IsHostRequirement());
				});
			});

			services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();
			
			return services;
		}
	}
}
