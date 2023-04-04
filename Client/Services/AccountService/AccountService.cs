using Client.Models;
using Client.Models.DTOs;
using System.Net.Http.Json;

namespace Client.Services.AccountService
{
   

    public class AccountService : IAccountService
    {
		
		private readonly HttpClient _http;
		private readonly IConfiguration _config;
		string BASE_URL = "https://localhost:7210/";

		public AccountService(HttpClient http, IConfiguration config)
		{
			_config = config;       
            _http = http;
            //BASE_URL = _config["BaseUrl"];

		}

        public Task<Result<bool>> FacebookLogin()
        {
            throw new NotImplementedException();
        }

        public Task<Result<UserDTO>> GetCurrentUser()
        {
            throw new NotImplementedException();
        }

        public async Task<Result<bool>> Login(LoginDTO loginDTO)
        {
            var result = await _http.PostAsJsonAsync(BASE_URL + "api/Account/login", loginDTO);
            return await result.Content.ReadFromJsonAsync<Result<bool>>();
        }

        public Task<Result<UserDTO>> RefreshToken()
        {
            throw new NotImplementedException();
        }

        public async Task<Result<bool>> Register(RegisterDTO registerDTO)
        {
            var result = await _http.PostAsJsonAsync(BASE_URL +"api/Account/register", registerDTO);
            return await result.Content.ReadFromJsonAsync<Result<bool>>();
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
