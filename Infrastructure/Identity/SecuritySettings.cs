namespace Infrastructure.Identity
{
	public class SecuritySettings
	{
        public string Provider { get; set; }
        public bool RequireConfirmedAccount { get; set; }
		public bool RequireConfirmedEmail { get; set; }
		public bool Lockout { get; set; }
		public bool LockoutAllowedForNewUsers { get; set; }
		public int LockoutMaxFailedAccessAttempts { get; set; }
		public int LockoutDefaultLockoutTimeSpan { get; set; }
		public int PasswordRequiredUniqueChars { get; set; }
		public bool PasswordRequireNonAlphanumeric { get; set; }
		public bool PasswordRequireDigit { get; set; }
		public int PasswordRequiredLength { get; set; }
		public bool PasswordRequireUppercase { get; set; }
		public bool PasswordRequireLowercase { get; set; }
		public bool PasswordRequireUniqueEmail { get; set; }	
		public JwtSettings JwtSettings { get; set; }
		public AzureAd AzureAd { get; set; }
		public Swagger Swagger { get; set; }                          
     
	}

    public class JwtSettings
	{
        public string? key { get; set; }
        public int tokenExpirationInMinutes { get; set; }
		public int refreshTokenExpirationInDays { get; set; }
		public int refreshTokenRemoveInDays { get; set; }	
		public string? validIssuer { get; set; }
		public string? validAudience { get; set; }	
	}

	public class AzureAd
	{
		public string? Instance { get; set; }
		public string? Domain { get; set; }
		public string? TenantId { get; set; }
		public string? ClientId { get; set; }
		public string? Scopes { get; set; }
		public string? RootIssuer { get; set; }
	}

	public class Swagger
	{
		public string? AuthorizationUrl { get; set; }
		public string? TokenUrl { get; set; }
		public string? ApiScope { get; set; }
		public string? OpenIdClientId { get; set; }		
	}
}
