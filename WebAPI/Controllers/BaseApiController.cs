using WebAPI.Extensions;
using Application.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.HttpExtensions;

namespace API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BaseApiController : ControllerBase
	{
		//private IMediator _mediator;
		//protected IMediator Mediator => _mediator ??= HttpContext.RequestServices
		//	.GetService<IMediator>();

		protected ActionResult HandleResult<T>(Result<T> result)
		{
			if (result == null) return NotFound();
			if (result.IsSuccess && result.Data != null)
				return Ok(result.Data);
			if (result.IsSuccess && result.Data == null)
				return NotFound();
			return BadRequest(result.Errors);
		}

		protected ActionResult HandlePagedResult<T>(Result<PagedList<T>> result)
		{
			if (result == null) return NotFound();
			if (result.IsSuccess && result.Data != null)
			{
				Response.AddPaginationHeader(result.Data.CurrentPage, result.Data.PageSize, result.Data.TotalCount, result.Data.TotalPages);
				return Ok(result.Data);
			}
			if (result.IsSuccess && result.Data == null)
				return NotFound();
			return BadRequest(result.Errors);
		}
	}
}
