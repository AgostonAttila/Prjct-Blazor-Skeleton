namespace Domain
{
	public class RefreshToken
	{
		public int Id { get; set; }
		public string? Token { get; set; }
		public DateTime Expires { get; set; } = DateTime.UtcNow.AddDays(7);
		public DateTime Created { get; set; }
		public string? CreatedByIp { get; set; }	


		public AppUser AppUser { get; set; }
					
		
		public DateTime? Revoked { get; set; }
		public string? RevokedByIp { get; set; }
		public string? ReplacedByToken { get; set; }
		public string? ReasonRevoked { get; set; }


		public bool IsActive => Revoked == null && !IsExpired;
		public bool IsExpired => DateTime.UtcNow >= Expires;
		public bool IsRevoked => Revoked != null;
	}
}
