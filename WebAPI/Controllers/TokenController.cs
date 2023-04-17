using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using API.Controllers;
using WebAPI.Services;
using Application.Core;
using Domain.DTOs;

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

			var name = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);

			var refreshToken = Request.Cookies["refreshToken"];
			var user = await _userManager.Users
				.Include(r => r.RefreshTokens)
				.Include(p => p.Photos)
				.FirstOrDefaultAsync(x => x.Email == name);

			if (user == null) return Unauthorized();

			var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

			if (oldToken != null && !oldToken.IsActive) return Unauthorized();

			//await _userManager.UpdateAsync(user);
			UserDTO userDTO = _tokenService.CreateUserObject(user);
			return Ok(new Result<string> { IsSuccess = true, Value = userDTO.Token });
		}







	}
}
