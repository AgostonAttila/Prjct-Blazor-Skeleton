using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Writers;
using Persistence;
using WebAPI.Extensions;
using static Org.BouncyCastle.Math.EC.ECCurve;

var builder = WebApplication.CreateBuilder(args);

//services
builder.Services.AddControllers(opt =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
});
//.AddFluentValidation(config =>
//{
//    config.RegisterValidatorsFromAssemblyContaining<Create>();
//});

builder.Services.AddDbContextServiceExtensions(builder.Configuration);

//builder.Services.AddDbContext<DataContext>(options =>
//{
//	options.UseSqlServer("Server=./;Database=skeleton;User Id=sa3;Password=Titkos!;MultipleActiveResultSets=True;");
//});

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);


builder.Services.AddHttpContextAccessor();

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

//biztonsági szûrések XSS stb

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
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

app.UseCors("CorsPolicy");

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


app.MapControllers();
app.MapHub<ChatHub>("/chat");
app.MapFallbackToController("Index", "Fallback");


await app.MigrateDatabase().RunAsync();
