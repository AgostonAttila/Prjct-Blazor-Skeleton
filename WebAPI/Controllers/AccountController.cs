using WebAPI.Services;
using Infrastructure.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using Domain.DTOs;
using Application.Core;
using API.Controllers;

namespace WebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : BaseApiController
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ITokenService _tokenService;
		private readonly EmailSender _emailSender;

		private readonly IConfiguration _config;
		private readonly HttpClient _httpClient;	
		private readonly SignInManager<AppUser> _signInManager;


		//private readonly IMapper _automapper;

		public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IConfiguration config, EmailSender emailSender, IHttpContextAccessor httpContextAccessor)
		{
			_emailSender = emailSender;
			_config = config;
			_tokenService = tokenService;
			_signInManager = signInManager;
			_userManager = userManager;
			_httpClient = new HttpClient
			{
				BaseAddress = new System.Uri("https://graph.facebook.com")
			};			
		}

		[AllowAnonymous]
		[HttpPost("login")]
		public async Task<ActionResult<Result<UserDTO>>> Login(LoginDTO loginDTO)
		{

			var user = await _userManager.Users.Include(o=>o.RefreshTokens)  //.FindByEmailAsync(LoginDTO.Email);
			.FirstOrDefaultAsync(x => x.Email == loginDTO.Email);


			if (user == null) return Unauthorized("Invalid email");
			//if (!user.EmailConfirmed) return Unauthorized("Email not confirmed");
			//var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
			//if (result.Succeeded)
			{
				await SetRefreshToken(user);
				UserDTO userDTO = _tokenService.CreateUserObject(user);

				_tokenService.RemoveOldRefreshTokens(user);


				await _userManager.UpdateAsync(user);
				return new Result<UserDTO> { IsSuccess = true, Data = userDTO };
			}

			

			return Unauthorized("Invalid pwd");
		}		

		//[AllowAnonymous]
		//[HttpPost("fbLogin")]
		//public async Task<ActionResult<UserDTO>> FacebookLogin(string accessToken)
		//{
		//	var fbVerifyKeys = _config["Facebook:AppId"] + "|" + _config["Facebook:AppSecret"];

		//	var verifyToken = await _httpClient
		//		.GetAsync($"debug_token?input_token={accessToken}&access_token={fbVerifyKeys}");

		//	if (!verifyToken.IsSuccessStatusCode) return Unauthorized();

		//	var fbUrl = $"me?access_token={accessToken}&fields=name,email,picture.width(100).height(100)";

		//	var response = await _httpClient.GetAsync(fbUrl);

		//	if (!response.IsSuccessStatusCode) return Unauthorized();

		//	var fbInfo = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

		//	var username = (string)fbInfo.id;

		//	var user = await _userManager.Users.Include(p => p.Photos)
		//		.FirstOrDefaultAsync(x => x.UserName == username);

		//	if (user != null) return CreateUserObject(user);

		//	user = new AppUser
		//	{
		//		DisplayName = (string)fbInfo.name,
		//		Email = (string)fbInfo.email,
		//		UserName = (string)fbInfo.id,
		//		Bio = " ",
		//		Photos = new List<Photo>
		//	{
		//		new Photo
		//		{
		//			Id = "fb_" + (string)fbInfo.id,
		//			Url = (string)fbInfo.picture.data.url,
		//			IsMain = true
		//		}}
		//	};

		//	user.EmailConfirmed = true;

		//	var result = await _userManager.CreateAsync(user);

		//	if (!result.Succeeded) return BadRequest("Problem creating user account");
		//	await SetRefreshToken(user);
		//	return CreateUserObject(user);
		//}

		[AllowAnonymous]
		[HttpPost("register")]
		public async Task<ActionResult<string>> Register(RegisterDTO registerDTO)
		{

			if (registerDTO == null || !ModelState.IsValid)
				return BadRequest();

			if (await _userManager.Users.AnyAsync(x => x.Email == registerDTO.Email))
			{
				ModelState.AddModelError("email", "Email taken");
				//sendAlreadyRegisteredEmail
				return ValidationProblem("Email taken");
			}
			if (await _userManager.Users.AnyAsync(x => x.UserName == registerDTO.Username))
			{
				ModelState.AddModelError("username", "Username taken");
				return ValidationProblem("Username taken");
			}

			var user = new AppUser
			{
				DisplayName = registerDTO.DisplayName,
				Email = registerDTO.Email,
				UserName = registerDTO.Username,
				Created = DateTime.UtcNow,
				Bio = " "
			};

			//var user = _automapper.Map<AppUser>(registerDTO);

			var result = await _userManager.CreateAsync(user, registerDTO.Password);

			/*if (result.Succeeded)
            {
                await SetRefreshToken(user);
                return CreateUserObject(user);
            }            
             return BadRequest("Problem registering user");
            */

			if (!result.Succeeded)
			{
				var errors = result.Errors.Select(e => e.Description);
				return BadRequest(new Result<string> { Errors = new List<string> { "Problem registering user" } });
			}

			await _userManager.AddToRoleAsync(user, "Viewer");
			//await SendVerificationEmail(user);

			return Ok(new Result<string> { IsSuccess = true, Data = "Registration success - please verify email"  });


		}

		[AllowAnonymous]
		[HttpPost("verifyEmail")]
		public async Task<IActionResult> VerifyEmail(string token, string email)
		{
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null) return Unauthorized();
			var decodedTokenBytes = WebEncoders.Base64UrlDecode(token);
			var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);
			var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

			if (!result.Succeeded) return BadRequest("Could not verify email address");

			return Ok("Email confirmed - you can now login");
		}

		[AllowAnonymous]
		[HttpGet("resendEmailConfirmationLink")]
		public async Task<IActionResult> ResendEmailConfirmationLink(string email)
		{
			var user = await _userManager.FindByEmailAsync(email);

			if (user == null) return Unauthorized();

			var origin = Request.Headers["origin"];
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

			var verifyUrl = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";
			var message = $"<p>Please click the below link to verify your email address:</p><p><a href='{verifyUrl}'>Click to verify email</a></p>";

			await _emailSender.SendEmailAsync(user.Email, "Please verify email", message);

			return Ok("Email verification link resent");
		}
		
		[Authorize]
		[HttpPost("logout")]
		public async Task<ActionResult<string>> Logout()
		{
			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Expires = DateTime.UtcNow.AddDays(7)
			};

			Response.Cookies.Delete("refreshToken", cookieOptions);

			return Ok(new Result<string> { IsSuccess = true, Data = "Logout success" });
		}


		//public async Task<ActionResult<UserDTO>> GetCurrentUser()
		//{
		//	var user = await _userManager.Users.Include(p => p.Photos)
		//	.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));      //.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
		//	await SetRefreshToken(user); // nem biztos h kell ide mert csak frissiti a böngészőt
		//	return CreateUserObject(user);
		//}
	
		private async Task SetRefreshToken(AppUser user)
		{
			var refreshToken = _tokenService.GenerateRefreshToken(ipAddress());

			user.RefreshTokens.Add(refreshToken);
			await _userManager.UpdateAsync(user);

			var cookieOptions = new CookieOptions
			{
				HttpOnly = true,
				Expires = DateTime.UtcNow.AddDays(7)
			};

			Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
		}
			
		private async Task SendVerificationEmail(AppUser user)
		{
			var origin = Request.Headers["origin"];
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

			var verifyUrl = $"{origin}/account/verifyEmail?token={token}&email={user.Email}";
			var message = $"<p>Please click the below link to verify your email address:</p><p><a href='{verifyUrl}'>Click to verify email</a></p>";

			await _emailSender.SendEmailAsync(user.Email, "Please verify email", message);
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
