using Infrastructure.Logging;
using Microsoft.AspNetCore.Identity;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity.Seed_Identity
{
	internal class SeedClaims
	{
		public static async Task SeedIdentityClaims(RoleManager<ApplicationRole> roleManager)
		{
			try
			{

				
			}
			catch (Exception e)
			{
				StaticLogger.EnsureInitialized();
				Log.Error(e, "Seeding Role error");
			}
		}
	}
}
