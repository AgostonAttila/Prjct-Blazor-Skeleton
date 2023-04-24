using System.Text;
using Domain;
using Infrastructure.BackgroundJobs;
using Infrastructure.Identity;
using Infrastructure.Logging;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using Serilog;

namespace WebAPI.Extensions
{
	public static class ServiceExtensions
	{
		public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
		{
			var securitySettings = config.GetSection("SecuritySettings").Get<SecuritySettings>();
			if (securitySettings is null) throw new Exception("SecuritySettings Provider is not configured.");
			if (string.IsNullOrEmpty(securitySettings.JwtSettings.key)) throw new Exception("SecuritySettings is not configured.");
				
		

		
			services.AddIdentityCore<AppUser>(opt =>
			{
				opt.Lockout.AllowedForNewUsers = securitySettings.LockoutAllowedForNewUsers;
				opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(securitySettings.LockoutDefaultLockoutTimeSpan);
				opt.Lockout.MaxFailedAccessAttempts = securitySettings.LockoutMaxFailedAccessAttempts;
				opt.Password.RequireNonAlphanumeric = securitySettings.PasswordRequireNonAlphanumeric;
				opt.Password.RequiredUniqueChars = securitySettings.PasswordRequiredUniqueChars;
				opt.Password.RequireDigit = securitySettings.PasswordRequireDigit;
				opt.Password.RequiredLength = securitySettings.PasswordRequiredLength;
				opt.Password.RequireUppercase = securitySettings.PasswordRequireUppercase;
				opt.Password.RequireLowercase = securitySettings.PasswordRequireLowercase;
				opt.SignIn.RequireConfirmedEmail = securitySettings.RequireConfirmedEmail;
				opt.User.RequireUniqueEmail = securitySettings.PasswordRequireUniqueEmail;			
			})
			.AddRoles<IdentityRole>()
			.AddEntityFrameworkStores<DataContext>()
			.AddSignInManager<SignInManager<AppUser>>()
			//.AddRoleManager<RoleManager<ApplicationRole>>()
			.AddDefaultTokenProviders();

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securitySettings.JwtSettings.key));

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

			//services.AddTransient<IAuthorizationHandler, IsHostRequirementHandler>();

			StaticLogger.EnsureInitialized();
			Log.Information($"AddIdentityCore  jwt token expires: {securitySettings.JwtSettings.tokenExpirationInMinutes } minutes , refresh token expires: {securitySettings.JwtSettings.refreshTokenExpirationInDays} , refresh token TTL: {securitySettings.JwtSettings.refreshTokenRemoveInDays} ");

			return services;
		}
	}
}
