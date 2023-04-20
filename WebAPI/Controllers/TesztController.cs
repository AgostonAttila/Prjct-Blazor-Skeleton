using API.Controllers;
using Microsoft.AspNetCore.Mvc;
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

			return Ok(new Result<string> { IsSuccess = true, Data = "OK tetsz1" });
		}

		[Authorize]
		[HttpGet("teszt2")]
		public async Task<ActionResult<Result<string>>> Teszt2()
		{

			return Ok(new Result<string> { IsSuccess = true, Data = "OK tetsz2" });
		}
	}
}
