using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Application.Core;
using Domain;
using Domain.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
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
		private readonly IConfiguration _config;
		private readonly IConfigurationSection _jwtSettings;
		private readonly UserManager<AppUser> _userManager;
		//private readonly AppSettings _appSettings;

		public TokenService(IConfiguration config, UserManager<AppUser> userManager)//, AppSettings appSettings)
		{
			_config = config;
			_jwtSettings = _config.GetSection("JwtSettings");
			_userManager = userManager;
			//_appSettings = appSettings;
		}

		public string CreateToken(AppUser user)
		{
			var claims = GetClaims(user).Result;

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Audience = _jwtSettings.GetSection("validAudience").Value,
				Issuer = _jwtSettings.GetSection("validIssuer").Value,
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings.GetSection("expiryInMinutes").Value)),
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
				Expires = DateTime.Now.AddDays(7),
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
			var key = Encoding.UTF8.GetBytes(_jwtSettings.GetSection("securityKey").Value);
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
				issuer: _jwtSettings.GetSection("validIssuer").Value,
				audience: _jwtSettings.GetSection("validAudience").Value,
				claims: claims,
				expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings.GetSection("expiryInMinutes").Value)),
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
					Encoding.UTF8.GetBytes(_jwtSettings.GetSection("securityKey").Value)),
				ValidateLifetime = false,
				ValidIssuer = _jwtSettings.GetSection("validIssuer").Value,
				ValidAudience = _jwtSettings.GetSection("validAudience").Value,
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
			  x.Created.AddDays(2) <= DateTime.UtcNow);
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
