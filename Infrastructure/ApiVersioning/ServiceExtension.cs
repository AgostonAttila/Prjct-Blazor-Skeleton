using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.ApiVersioning
{
	public static class ServiceExtension
	{

		public static IServiceCollection AddApiVersioning(this IServiceCollection services) =>
	   services.AddApiVersioning(config =>
	   {
		   config.DefaultApiVersion = new ApiVersion(1, 0);
		   config.AssumeDefaultVersionWhenUnspecified = true;
		   config.ReportApiVersions = true;
	   });
	}
}
