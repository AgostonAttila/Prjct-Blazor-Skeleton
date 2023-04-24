using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
	public static class ClaimsPrincipalExtensions
	{
		public static string? GetEmail(this ClaimsPrincipal principal)
			=> principal.FindFirstValue(ClaimTypes.Email);
	
		public static string? GetFullName(this ClaimsPrincipal principal)
			=> principal?.FindFirst(OwnClaims.Fullname)?.Value;

		public static string? GetFirstName(this ClaimsPrincipal principal)
			=> principal?.FindFirst(ClaimTypes.Name)?.Value;

		public static string? GetSurname(this ClaimsPrincipal principal)
			=> principal?.FindFirst(ClaimTypes.Surname)?.Value;

		public static string? GetPhoneNumber(this ClaimsPrincipal principal)
			=> principal.FindFirstValue(ClaimTypes.MobilePhone);

		public static string? GetUserId(this ClaimsPrincipal principal)
		   => principal.FindFirstValue(ClaimTypes.NameIdentifier);

		public static string? GetImageUrl(this ClaimsPrincipal principal)
		   => principal.FindFirstValue(OwnClaims.ImageUrl);

		public static DateTimeOffset GetExpiration(this ClaimsPrincipal principal) =>
			DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(
				principal.FindFirstValue(OwnClaims.Expiration)));

		private static string? FindFirstValue(this ClaimsPrincipal principal, string claimType) =>
			principal is null
				? throw new ArgumentNullException(nameof(principal))
				: principal.FindFirst(claimType)?.Value;
	}
}
