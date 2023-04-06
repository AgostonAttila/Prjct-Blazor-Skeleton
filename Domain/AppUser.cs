using Microsoft.AspNetCore.Identity;

namespace Domain
{
	public class AppUser : IdentityUser
	{
		public string DisplayName { get; set; }
		public string Bio { get; set; }		
		public ICollection<Photo> Photos { get; set; }
		public RefreshToken RefreshToken { get; set; }
		//public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();	
	}
}
