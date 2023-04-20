using Application.Interfaces;
using Infrastructure.Email;
using Infrastructure.Security;
using WebAPI.Services;

namespace WebAPI.Extensions
{
	public static class ApplicationServiceExtension
	{				
		public static IServiceCollection AddOwnServices(this IServiceCollection services)			
		{
			services.AddScoped<ITokenService, TokenService>();
			//services.AddMediatR(typeof(List.Handler).Assembly);
			//services.AddAutoMapper(typeof(MappingProfiles).Assembly);
			services.AddScoped<IUserAccessor, UserAccessor>();	
			services.AddScoped<EmailSender>();
			services.AddSignalR();

			return services;
		}					
		
	}
}


