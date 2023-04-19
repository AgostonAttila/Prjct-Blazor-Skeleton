using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Core;
using Application.Core.Exceptions;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace WebAPI.Middleware
{
    public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		//private readonly ILogger<ExceptionMiddleware> _logger;
		private readonly IHostEnvironment _env;
		public ExceptionMiddleware(RequestDelegate next,/* ILogger<ExceptionMiddleware> logger*/IHostEnvironment env)
		{
			_env = env;
			//_logger = logger;
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				//_logger.LogError(ex, ex.Message);
				Log.Error(ex,ex.Message);
				context.Response.ContentType = "application/json";
				var responseModel = new Result<string>() { IsSuccess = false, Message = ex?.Message };

				switch (ex)
				{
					case AppException e:						
						context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
						break;
					case ValidationException e:						
						context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
						responseModel.Errors = e.Errors;
						break;
					case KeyNotFoundException e:					
						context.Response.StatusCode = (int)HttpStatusCode.NotFound;
						break;
					default:						
						context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
						break;
				}

				var response = _env.IsDevelopment()
					? new AppException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
					: new AppException(context.Response.StatusCode, "Server Error");

				var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

				var json = JsonSerializer.Serialize(response, options);

				await context.Response.WriteAsync(json);
			}
		}
	}
}
