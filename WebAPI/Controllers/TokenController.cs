using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using API.Controllers;
using WebAPI.Services;
using Application.Core;
using Domain.DTOs;
using Infrastructure.Identity;

namespace WebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TokenController : BaseApiController
	{

		private readonly UserManager<AppUser> _userManager;
		private readonly ITokenService _tokenService;
		private readonly IHttpContextAccessor _httpContextAccessor;
				

		public TokenController(UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor,ITokenService tokenService)
		{
			_httpContextAccessor = httpContextAccessor;
			_userManager = userManager;
			_tokenService = tokenService;
		}


		[Authorize]
		[HttpPost("refreshToken")]
		public async Task<ActionResult<string>> RefreshToken()
		{

			var name =     _httpContextAccessor.HttpContext?.User.GetFirstName();

			var refreshToken = Request.Cookies["refreshToken"];
			var user = await _userManager.Users
				.Include(r => r.RefreshTokens)			
				.FirstOrDefaultAsync(x => x.Email == name);

			if (user == null) return Unauthorized();

			var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

			if (oldToken.IsRevoked)
			{			
				_tokenService.RevokeDescendantRefreshTokens(oldToken, user, ipAddress(), $"Attempted reuse of revoked ancestor token: {oldToken}");
				await _userManager.UpdateAsync(user);
			}

			if (oldToken != null && !oldToken.IsActive) return Unauthorized();
			
			var newRefreshToken = _tokenService.RotateRefreshToken(oldToken, ipAddress());
			user.RefreshTokens.Add(newRefreshToken);

			_tokenService.RemoveOldRefreshTokens(user);

			await _userManager.UpdateAsync(user);

			
			UserDTO userDTO = _tokenService.CreateUserObject(user);
			return Ok(new Result<string> { IsSuccess = true, Data = userDTO.Token });
		}

		[Authorize]
		[HttpPost("revokeToken")]
		public async Task<IActionResult> RevokeToken(string refreshToken)
		{		
			var token = refreshToken ?? Request.Cookies["refreshToken"];

			if (string.IsNullOrEmpty(token))
				return BadRequest(new { message = "Token is required" });

			var name = _httpContextAccessor.HttpContext?.User.GetFirstName();
			var user = await _userManager.Users
					.Include(r => r.RefreshTokens)
					.FirstOrDefaultAsync(x => x.Email == name);			

			if (user == null || (user.RefreshTokens.Any(x => x.Token == refreshToken) && _httpContextAccessor.HttpContext.User.Claims.Any(p=>p.Value != "Administrator")))
				return Unauthorized(new { message = "Unauthorized" });

			_tokenService.RevokeToken(token, ipAddress());
			return Ok(new { message = "Token revoked" });
		}

		private string ipAddress()
		{			
			if (Request.Headers.ContainsKey("X-Forwarded-For"))
				return Request.Headers["X-Forwarded-For"];
			else
				return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
		}
	}
}
