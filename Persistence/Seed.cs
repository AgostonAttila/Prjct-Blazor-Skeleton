using Domain;
using Microsoft.AspNetCore.Identity;

namespace Persistence
{
	public class Seed
	{


		public static async Task SeedData(DataContext context, UserManager<AppUser> userManager)
		{
			try
			{
				var users = new List<AppUser>
				{
					new AppUser
					{
						DisplayName = "Bob",
						UserName = "bob",
						Email = "bob@test.com",
						Bio = "Bio"
					},
					new AppUser
					{
						DisplayName = "Jane",
						UserName = "jane",
						Email = "jane@test.com",
						Bio = "Bio"
					},
					new AppUser
					{
						DisplayName = "Tom",
						UserName = "tom",
						Email = "tom@test.com",
						Bio = "Bio"
					},
				};

				foreach (var user in users)
				{
					await userManager.CreateAsync(user, "Pa$$w0rd");
				}
				await context.SaveChangesAsync();
			}
			catch (Exception e)
			{

				//throw new AppException() { Message = e.Message , StatusCode = 400 , Details = e.InnerException.Message } ;
			}
		}
	}
}
