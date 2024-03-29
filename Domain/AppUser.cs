﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
	public class AppUser : IdentityUser
	{
		public string DisplayName { get; set; }
		public string? Bio { get; set; }
		public DateTime? Created { get; set; }
		public ICollection<Photo> Photos { get; set; }			
		public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();	
	}
}
