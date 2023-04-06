namespace WebAPI.Extensions
{
	public static class MigrateDatabaseExtensions
	{
		public static  IHost MigrateDatabase(this IHost host)
		{		
			using (var scope = host.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				using (var appContext = scope.ServiceProvider.GetRequiredService<DataContext>())
				{
					try
					{
						var userManager = services.GetRequiredService<UserManager<AppUser>>();
						appContext.Database.MigrateAsync();
						Seed.SeedData(appContext, userManager);
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
