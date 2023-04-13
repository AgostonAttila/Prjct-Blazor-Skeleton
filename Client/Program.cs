using Blazored.LocalStorage;
using Client;
using Client.Services;
using Client.Services.AccountService;
using Client.Services.TesztService;
using Client.StateContainers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Syncfusion.Blazor;
using System.Net.Http.Headers;
using Toolbelt.Blazor;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClientInterceptor();
builder.Services.AddScoped(sp => new HttpClient
{
	BaseAddress = new Uri("https://localhost:7210/api/")
	//BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
}
.EnableIntercept(sp));


//Services
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITesztService, TesztService>();

builder.Services.AddScoped<RefreshTokenService>();


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

var host =  builder.Build();
SubscribeHttpClientInterceptorEvents2(host);
await host.RunAsync();


static void SubscribeHttpClientInterceptorEvents(WebAssemblyHost host)
{
	// Subscribe IHttpClientInterceptor's events.
	var httpInterceptor = host.Services.GetService<IHttpClientInterceptor>();
	httpInterceptor.BeforeSend += OnBeforeSend;
	httpInterceptor.AfterSendAsync += OnAfterSendAsync;
}

static void SubscribeHttpClientInterceptorEvents2(WebAssemblyHost host)
{
	// Subscribe IHttpClientInterceptor's events.
	HttpInterceptorService httpInterceptor = host.Services.GetService<HttpInterceptorService>();
	httpInterceptor.RegisterEvent();
	
}

 static void OnBeforeSend(object sender, HttpClientInterceptorEventArgs args)
{
	//Console.WriteLine("BeforeSend event of HttpClientInterceptor");
	//Console.WriteLine($"  - {args.Request.Method} {args.Request.RequestUri}");	
}

 static async Task OnAfterSendAsync(object sender, HttpClientInterceptorEventArgs args)
{
	//Console.WriteLine("AfterSend event of HttpClientInterceptor");
	//Console.WriteLine($"  - {args.Request.Method} {args.Request.RequestUri}");
	//Console.WriteLine($"  - HTTP Status {args.Response?.StatusCode}");

	//var capturedContent = await args.GetCapturedContentAsync();

	//Console.WriteLine($"  - Content Headers");
	//foreach (var headerText in capturedContent.Headers.Select(h => $"{h.Key}: {string.Join(", ", h.Value)}"))
	//{
	//	Console.WriteLine($"    - {headerText}");
	//}

	//var httpContentString = await capturedContent.ReadAsStringAsync();
	//Console.WriteLine($"  - HTTP Content \"{httpContentString}\"");
}