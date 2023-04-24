using Infrastructure.Identity;

namespace WebAPI.Extensions
{
	public static class MigrateDatabaseExtension
	{
		public static IHost MigrateDatabase(this IHost host)
		{
			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				using (var appContext = scope.ServiceProvider.GetRequiredService<DataContext>())
				{
					try
					{
						var userManager = services.GetRequiredService<UserManager<AppUser>>();
						//var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
						appContext.Database.EnsureCreatedAsync().Wait();
						appContext.Database.MigrateAsync();

						bool isAnyUser = userManager.Users.Any();
						if (!isAnyUser)
						{
							//SeedRoles.SeedIdentityRoles(roleManager);
							Seed.SeedData(appContext,userManager);
						}
					}
					catch (Exception ex)
					{
						var logger = services.GetRequiredService<ILogger<Program>>();
						logger.LogError(ex, "An error occured during migraiton");
					}
				}
			}

			return host;
		}
	}
}
