using Infrastructure.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;

namespace Infrastructure.Identity
{
	public static class SeedRoles
	{
		public static async Task SeedIdentityRoles(RoleManager<ApplicationRole> roleManager)
		{
			try
			{

				bool isAnyRole = roleManager.Roles.Any();
				if (!isAnyRole)
				{
					foreach (string roleName in OwnRoles.DefaultRoles)
					{
						if (await roleManager.Roles.SingleOrDefaultAsync(r => r.Name == roleName)
							is not ApplicationRole role)
						{
							StaticLogger.EnsureInitialized();
							Log.Information("Seeding {role} Role", roleName);
							role = new ApplicationRole(roleName, $"{roleName} Role for App");
							await roleManager.CreateAsync(role);
						}

					}
				}
			}
			catch (Exception e)
			{
				StaticLogger.EnsureInitialized();
				Log.Error(e,"Seeding Role error");
			}
		}
	}
}
