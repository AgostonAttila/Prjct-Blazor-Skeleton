
using Infrastructure.BackgroundJobs;
using Infrastructure.Caching;
using Infrastructure.Cors;
using Infrastructure.HealthCheck;
using Infrastructure.Logging;
using Infrastructure.Middlaware;
using Infrastructure.SignalR;
using Microsoft.Extensions.FileProviders;
using Serilog;
using WebAPI.Configurations;



StaticLogger.EnsureInitialized();
Log.Information("Server Booting Up...");
try
{
	var builder = WebApplication.CreateBuilder(args);	
	builder.AddConfigurations().RegisterSerilog();


	builder.Services.AddControllers(opt =>
	{
		var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
		opt.Filters.Add(new AuthorizeFilter(policy));
	});
	//.AddFluentValidation(config =>
	//{
	//    config.RegisterValidatorsFromAssemblyContaining<Create>();
	//});


	//compress + 2FA + mail + reset pwd +törlés cookieból bezáráskor
	//cahce hangfire localization notification olvasni 
	//api versioning

	////.AddApiVersioning()
	builder.Services.AddIdentityServices(builder.Configuration);
	builder.Services.AddSwaggerExtension();
	builder.Services.AddAppSettings(builder.Configuration);
	builder.Services.AddBackgroundJobs(builder.Configuration);
	builder.Services.AddCaching(builder.Configuration);
	builder.Services.AddCorsPolicy();
	//builder.Services.AddExceptionMiddleware();
	////.AddBehaviours(applicationAssembly)
	builder.Services.AddHealthChecks();
	///////AddPOLocalization(config)
	////.AddMailing(config)
	////.AddMediatR(Assembly.GetExecutingAssembly())	
	////.AddNotifications(config)
	////.AddOpenApiDocumentation(config)
	builder.Services.AddPersistence(builder.Configuration);
	////builder.Services.AddRequestLogging(config)
	builder.Services.AddRouting(options => options.LowercaseUrls = true);	
	builder.Services.AddOwnServices();
	builder.Services.AddHttpContextAccessor();

	var app = builder.Build();


	app.UseExceptionMiddleware();

	//biztonsági szûrések XSS stb

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.UseUseSwaggerExt();
		app.UseDeveloperExceptionPage();
	}
	else
	{
		app.Use(async (context, next) =>
		{
			context.Response.Headers.Add("Strict-Transport-Security", "max-age=315");
			await next.Invoke();
		});
	}

	app.UseCorsPolicy();

	app.UseDefaultFiles();
	app.UseStaticFiles();
	app.UseStaticFiles(new StaticFileOptions()
	{
		FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"StaticFiles")),
		RequestPath = new PathString("/StaticFiles")
	});

	

	app.UseAuthentication();
	app.UseAuthorization();

	//app.UseHttpsRedirection();

	app.UseHealthChecks();

	app.MapControllers();
	app.MapHub<ChatHub>("/chat");
	app.MapFallbackToController("Index", "Fallback");


	await app.MigrateDatabase().RunAsync();
}
catch (Exception ex) when (!ex.GetType().Name.Equals("HostAbortedException", StringComparison.Ordinal))
{
	StaticLogger.EnsureInitialized();
	Log.Fatal(ex, "Unhandled exception");
}
finally
{
	StaticLogger.EnsureInitialized();
	Log.Information("Server Shutting down...");
	Log.CloseAndFlush();
}
