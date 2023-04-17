using API.Controllers;
using Infrastructure.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Net.Http;
using WebAPI.Services;
using Application.Core;

namespace WebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TesztController : BaseApiController
	{
		public TesztController()
		{
			
		}

		[AllowAnonymous]
		[HttpGet("teszt1")]
		public async Task<ActionResult<Result<string>>> Teszt1( )
		{

			return Ok(new Result<string> { IsSuccess = true, Value = "OK tetsz1" });
		}

		[Authorize]
		[HttpGet("teszt2")]
		public async Task<ActionResult<Result<string>>> Teszt2()
		{

			return Ok(new Result<string> { IsSuccess = true, Value = "OK tetsz2" });
		}
	}
}
