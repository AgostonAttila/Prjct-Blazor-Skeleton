using Application.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Infrastructure.HealthCheck
{
	public static class ServiceExtensions
	{
		public static void UseHealthChecks(this IApplicationBuilder app)
		{
			// app.UseHealthChecks("/health");
			app.UseHealthChecks("/health", new HealthCheckOptions
			{
				ResponseWriter = async (context, report) =>
				{
					context.Response.ContentType = "application/json";
					var response = new HealthCheckReponse
					{
						Status = report.Status.ToString(),
						HealthChecks = report.Entries.Select(x => new IndividualHealthCheckResponse
						{
							Component = x.Key,
							Status = x.Value.Status.ToString(),
							Description = x.Value.Description

						}),
						HealthCheckDuration = report.TotalDuration
					};
					await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
				}
			});
		}
	}
}
