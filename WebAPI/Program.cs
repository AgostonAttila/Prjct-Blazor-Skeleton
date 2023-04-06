using Microsoft.EntityFrameworkCore;
using Persistence;
using WebAPI.Extensions;

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
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

//biztonsági szûrések XSS stb

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
}
else
{
    app.Use(async (context, next) =>
    {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=315");
        await next.Invoke();
    });
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

//app.UseHttpsRedirection();


app.MapControllers();
app.MapHub<ChatHub>("/chat");
app.MapFallbackToController("Index", "Fallback");


await app.MigrateDatabase().RunAsync();
