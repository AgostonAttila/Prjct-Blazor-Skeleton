using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Core.Exceptions;
using Domain.DTOs;
using Infrastructure.Identity;
using Microsoft.IdentityModel.Tokens;


namespace WebAPI.Services
{

	public interface ITokenService
	{
		string CreateToken(AppUser user);
		RefreshToken GenerateRefreshToken(string ipAddress);
		SigningCredentials GetSigningCredentials();
		Task<List<Claim>> GetClaims(AppUser user);
		JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims);
		ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
		UserDTO CreateUserObject(AppUser user);
		void RevokeToken(string token, string ipAddress);
		void RemoveOldRefreshTokens(AppUser user);
		void RevokeDescendantRefreshTokens(RefreshToken refreshToken, AppUser user, string ipAddress, string v);
		RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress);
	}

	public class TokenService : ITokenService
	{
	
		private readonly UserManager<AppUser> _userManager;
		private SecuritySettings _securitySettings;

		public TokenService(IConfiguration config, UserManager<AppUser> userManager)
		{					
			_userManager = userManager;

			_securitySettings = config.GetSection("SecuritySettings").Get<SecuritySettings>();
			if (_securitySettings is null) throw new Exception("SecuritySettings Provider is not configured.");
			if (string.IsNullOrEmpty(_securitySettings.JwtSettings.key)) throw new Exception("SecuritySettings is not configured.");
		}

		public string CreateToken(AppUser user)
		{
			var claims = GetClaims(user).Result;

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Audience = _securitySettings.JwtSettings.validAudience,
				Issuer = _securitySettings.JwtSettings.validIssuer,
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.Now.AddMinutes(Convert.ToDouble(_securitySettings.JwtSettings.tokenExpirationInMinutes)),
				SigningCredentials = GetSigningCredentials(),
			};

			//GenerateTokenOptions(SigningCredentials signingCredentials, List < Claim > claims)
			var tokenHandler = new JwtSecurityTokenHandler();

			var token = tokenHandler.CreateToken(tokenDescriptor);

			return tokenHandler.WriteToken(token);
		}

		public RefreshToken GenerateRefreshToken(string ipAddress)
		{

			return new RefreshToken
			{
				Expires = DateTime.Now.AddDays(_securitySettings.JwtSettings.refreshTokenExpirationInDays),
				Token = getUniqueToken(),
				Created = DateTime.UtcNow,
				CreatedByIp = ipAddress
			};


			string getUniqueToken()
			{

				var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
				var tokenIsUnique = true; //!_context.Users.Any(u => u.RefreshTokens.Any(t => t.Token == token));

				if (!tokenIsUnique)
					return getUniqueToken();

				return token;
			}
		}

		public SigningCredentials GetSigningCredentials()
		{
			var key = Encoding.UTF8.GetBytes(_securitySettings.JwtSettings.key);
			var secret = new SymmetricSecurityKey(key);

			return new SigningCredentials(secret, SecurityAlgorithms.HmacSha512Signature);
		}

		public async Task<List<Claim>> GetClaims(AppUser user)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Email)
			};

			var roles = await _userManager.GetRolesAsync(user);
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			return claims;
		}

		public JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
		{
			var tokenOptions = new JwtSecurityToken(
				issuer: _securitySettings.JwtSettings.validIssuer,
				audience: _securitySettings.JwtSettings.validAudience,
				claims: claims,
				expires: DateTime.Now.AddMinutes(Convert.ToDouble(_securitySettings.JwtSettings.tokenExpirationInMinutes)),
				signingCredentials: signingCredentials);

			return tokenOptions;
		}

		public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = true,
				ValidateIssuer = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(
					Encoding.UTF8.GetBytes(_securitySettings.JwtSettings.key)),
				ValidateLifetime = false,
				ValidIssuer = _securitySettings.JwtSettings.validIssuer,
				ValidAudience = _securitySettings.JwtSettings.validAudience,
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			SecurityToken securityToken;
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

			var jwtSecurityToken = securityToken as JwtSecurityToken;
			if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
				StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Invalid token");
			}

			return principal;
		}

		public UserDTO CreateUserObject(AppUser user)
		{

			//var signingCredentials = _tokenService.GetSigningCredentials();
			//var claims =  _tokenService.GetClaims(user);
			//var tokenOptions = _tokenService.GenerateTokenOptions(signingCredentials, claims.Result);
			//var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

			return new UserDTO
			{
				DisplayName = user.DisplayName,
				Image = user?.Photos?.FirstOrDefault(x => x.IsMain)?.Url,
				Token = CreateToken(user),
				Username = user.UserName
			};
		}

		public void RevokeToken(string token, string ipAddress)
		{		

			var user = GetUserByRefreshToken(token);
			var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

			if (!refreshToken.IsActive)
				throw new AppException(404,"Invalid token");
						
			RevokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
			_userManager.UpdateAsync(user);
			//_context.SaveChanges();
		}		

		public void RemoveOldRefreshTokens(AppUser user)
		{
			user.RefreshTokens.ToList().RemoveAll(x =>
			  !x.IsActive &&
			  x.Created.AddDays(_securitySettings.JwtSettings.refreshTokenRemoveInDays) <= DateTime.UtcNow);


			for (int i = 0; i < user.RefreshTokens.ToList().Count(); i++)
			{
				RefreshToken token = user.RefreshTokens.ToList()[i];

				if (token.IsRevoked)
				{ user.RefreshTokens.Remove(token); i--; }
			}
			
			//user.RefreshTokens.ToList().RemoveAll(x =>			 
			//  x.IsRevoked);
		}

		public void RevokeDescendantRefreshTokens(RefreshToken refreshToken, AppUser user, string ipAddress, string reason)
		{
			// recursively traverse the refresh token chain and ensure all descendants are revoked
			if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
			{
				var childToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
				if (childToken.IsActive)
					RevokeRefreshToken(childToken, ipAddress, reason);
				else
					RevokeDescendantRefreshTokens(childToken, user, ipAddress, reason);
			}
		}

		public RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
		{
			var newRefreshToken = GenerateRefreshToken(ipAddress);
			RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
			return newRefreshToken;
		}





		private AppUser GetUserByRefreshToken(string token)
		{
			List<AppUser> userList = _userManager.Users.Include(r => r.RefreshTokens).ToList();

			AppUser appUser = null;
			userList.ForEach(user => { if (user.RefreshTokens.Any(p => p.Token == token)) appUser = user; });

			return appUser;
		}

		private void RevokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
		{
			token.Revoked = DateTime.UtcNow;
			token.RevokedByIp = ipAddress;
			token.ReasonRevoked = reason;
			token.ReplacedByToken = replacedByToken;
		}

		
	}
}
