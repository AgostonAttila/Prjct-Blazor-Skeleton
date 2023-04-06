using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace WebAPI.Services
{
	public class TokenService
	{
		private readonly IConfiguration _config;
		private readonly IConfigurationSection _jwtSettings;
		private readonly UserManager<AppUser> _userManager;

		public TokenService(IConfiguration config, UserManager<AppUser> userManager)
		{
			_config = config;
			_jwtSettings = _config.GetSection("JwtSettings");
			_userManager = userManager;
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

		public RefreshToken GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomNumber);
			return new RefreshToken { Expires = DateTime.Now.AddDays(7), Token = Convert.ToBase64String(randomNumber) };
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
	}
}
