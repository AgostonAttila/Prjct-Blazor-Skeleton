using Blazored.LocalStorage;
using Client;
using Client.Services;
using Client.Services.AccountService;
using Client.StateContainers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{ 
	BaseAddress = new Uri("https://localhost:7210/api/")
}
.EnableIntercept(sp));



//Services
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddHttpClientInterceptor();

//LocalStorage
builder.Services.AddBlazoredLocalStorage();

//State containers
builder.Services.AddSingleton<AppState>();

//Authorization - Authentication
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<HttpInterceptorService>();

builder.Services.AddSyncfusionBlazor();

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration.GetSection("SyncFusion_API_KEY").Value);

await builder.Build().RunAsync();
