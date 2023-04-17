using Blazored.LocalStorage;
using Client.Models;
using Client.Models.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json.Linq;
using Syncfusion.Blazor.Kanban.Internal;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace Client.Services.AccountService
{


	public class AccountService : IAccountService
	{

		private readonly HttpClient _httpClient;
		private readonly AuthenticationStateProvider _authStateProvider;
		private readonly JsonSerializerOptions _options;
		private readonly ILocalStorageService _localStorageService;
		private NavigationManager _navigationManager;

		private static System.Timers.Timer refreshTokenTimer;


		public AccountService(HttpClient httpClient, AuthenticationStateProvider authStateProvider, ILocalStorageService localStorageService, NavigationManager navigationManager)
		{
			_httpClient = httpClient;
			_localStorageService = localStorageService;
			_navigationManager = navigationManager;
			_authStateProvider = authStateProvider;
			_options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
		}

		public Task<Result<UserDTO>> FacebookLogin()
		{
			throw new NotImplementedException();
		}

		public async Task<IIdentity> GetCurrentUser()
		{
			return (await _authStateProvider.GetAuthenticationStateAsync()).User.Identity;
		}

		public async Task<Result<UserDTO>> Login(LoginDTO loginDTO)
		{
			//         var result = await _http.PostAsJsonAsync("Account/login", loginDTO);

			//Result<UserDTO> response = await result.Content.ReadFromJsonAsync<Result<UserDTO>>();

			//if (response != null)
			//{
			//	await _localStorageService.SetItemAsync("authToken", response.Value.Token);
			//	await _authStateProvider.GetAuthenticationStateAsync();
			//	_navigationManager.NavigateTo("/");
			//}
			//else 
			//{ 

			//}
			//return await result.Content.ReadFromJsonAsync<Result<UserDTO>>();          	

			var content = JsonSerializer.Serialize(loginDTO);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

			var authResult = await _httpClient.PostAsync("Account/login", bodyContent);
			var authContent = await authResult.Content.ReadAsStringAsync();
			var result = JsonSerializer.Deserialize<Result<UserDTO>>(authContent, _options);

			if (!authResult.IsSuccessStatusCode)
				return result;

			await _localStorageService.SetItemAsync("authToken", result.Value.Token);
			((CustomAuthStateProvider)_authStateProvider).GetAuthenticationStateAsync();
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Value.Token);

			StartRefreshTokenTimer();

			return new Result<UserDTO> { IsSuccess = true };
		}

		public async Task<Result<string>> Logout()
		{
			//var authResult = await _httpClient.PostAsync("Account/logout", new StringContent("", Encoding.UTF8, "application/json"));
			//var authContent = await authResult.Content.ReadAsStringAsync();
			//var result = JsonSerializer.Deserialize<Result<string>>(authContent, _options);

			//if (!authResult.IsSuccessStatusCode)
			//	return result;

			await _localStorageService.RemoveItemAsync("authToken");

			((CustomAuthStateProvider)_authStateProvider).GetAuthenticationStateAsync();
			_httpClient.DefaultRequestHeaders.Authorization = null;

			StopRefreshTokenTimer();

			_navigationManager.NavigateTo("/");
			return new Result<string> { IsSuccess = true, Value = "Logout is Success" }; ;
		}

		public async Task<string> RefreshToken()
		{
			var token = await _localStorageService.GetItemAsync<string>("authToken");				
			
			var bodyContent = new StringContent("", Encoding.UTF8, "application/json");
			var refreshResult = await _httpClient.PostAsync("Token/refreshToken", bodyContent);

			if (!refreshResult.IsSuccessStatusCode)
				throw new ApplicationException("Something went wrong during the refresh token action");

			var refreshContent = await refreshResult.Content.ReadAsStringAsync();

			if (refreshContent != "''")
			{

				Result<string> result = JsonSerializer.Deserialize<Result<string>>(refreshContent, _options);

				await _localStorageService.SetItemAsync("authToken", result.Value);			
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", result.Value);
			}
			return "";
		}

		public async Task<Result<string>> Register(RegisterDTO registerDTO)
		{
			var content = JsonSerializer.Serialize(registerDTO);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

			var result = await _httpClient.PostAsJsonAsync("Account/register", registerDTO);
			Result<string> resultContent = await result.Content.ReadFromJsonAsync<Result<string>>();

			if (!resultContent.IsSuccess)
			{
				return resultContent;
			}

			return new Result<string> { IsSuccess = true };
		}

		public Task<Result<string>> ResendEmailConfirmationLink(string email)
		{
			throw new NotImplementedException();
		}

		public Task<Result<string>> VerifyEmail(string token, string email)
		{
			throw new NotImplementedException();
		}
		public void StartRefreshTokenTimer()
		{

			//const jwtToken = JSON.parse(atob(user.token.split(".")[1]));
			//const expires = new Date(jwtToken.exp * 1000);
			//const timeout = expires.getTime() - Date.now() - 60 * 1000;
			//this.refreshTokenTimeout = setTimeout(this.refreshToken, timeout);

			if (refreshTokenTimer is null)
			{
				refreshTokenTimer = new System.Timers.Timer(1000 * 60 * 4);
				refreshTokenTimer.Elapsed += (sender, args) =>
				{
					RefreshToken();
				};
				refreshTokenTimer.AutoReset = true;
				refreshTokenTimer.Enabled = true;
			}
		}

		public void StopRefreshTokenTimer()
		{
			if (refreshTokenTimer is not null)
			{
				refreshTokenTimer.Stop();
				refreshTokenTimer.Dispose();
				refreshTokenTimer = null;
			}
		}

		public void Dispose()
		{
			StopRefreshTokenTimer();
		}


	}



}
