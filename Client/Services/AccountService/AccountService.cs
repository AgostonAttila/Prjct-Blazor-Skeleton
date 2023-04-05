using Blazored.LocalStorage;
using Client.Models;
using Client.Models.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Principal;

namespace Client.Services.AccountService
{
   

    public class AccountService : IAccountService
    {

		private readonly HttpClient _http;
		private readonly AuthenticationStateProvider _authStateProvider;

		private readonly ILocalStorageService _localStorageService;
		private NavigationManager _navigationManager;

		private readonly IConfiguration _config;
		string BASE_URL = "https://localhost:7210/";

		public AccountService(HttpClient http, IConfiguration config, AuthenticationStateProvider authStateProvider, ILocalStorageService localStorageService, NavigationManager navigationManager)
		{
			_config = config;       
            _http = http;
			_localStorageService = localStorageService;
			_navigationManager = navigationManager;
			_authStateProvider = authStateProvider;		
			//BASE_URL = _config["BaseUrl"];

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
            var result = await _http.PostAsJsonAsync(BASE_URL + "api/Account/login", loginDTO);

			Result<UserDTO> response = await result.Content.ReadFromJsonAsync<Result<UserDTO>>();

			if (response != null)
			{
				await _localStorageService.SetItemAsync("authToken", response.Value.Token);
				await _authStateProvider.GetAuthenticationStateAsync();
				_navigationManager.NavigateTo("/");
			}
			else 
			{ 
			
			
			}
			return await result.Content.ReadFromJsonAsync<Result<UserDTO>>();          			
		}

		public async Task<bool> Logout()
		{
			await _localStorageService.RemoveItemAsync("authToken");
			_navigationManager.NavigateTo("/");
			return true;
		}

		public Task<Result<UserDTO>> RefreshToken()
        {
            throw new NotImplementedException();
        }

        public async Task<Result<string>> Register(RegisterDTO registerDTO)
        {
            var result = await _http.PostAsJsonAsync(BASE_URL +"api/Account/register", registerDTO);
            return await result.Content.ReadFromJsonAsync<Result<string>>();
        }

        public Task<Result<string>> ResendEmailConfirmationLink(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Result<string>> VerifyEmail(string token, string email)
        {
            throw new NotImplementedException();
        }
    }
}
