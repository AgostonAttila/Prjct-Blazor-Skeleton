using Client.Models;
using Client.Models.DTOs;
using System.Net.Http.Json;

namespace Client.Services.AccountService
{
    public class AccountService : IAccountService
    {

        private readonly HttpClient _http;

        public AccountService(HttpClient http)
        {
            _http = http;
        }

        public Task<Result<bool>> FacebookLogin()
        {
            throw new NotImplementedException();
        }

        public Task<Result<UserDTO>> GetCurrentUser()
        {
            throw new NotImplementedException();
        }

        public async Task<Result<bool>> Login(LoginDTO user)
        {
            var result = await _http.PostAsJsonAsync("api/account/login",  user);
            return await result.Content.ReadFromJsonAsync<Result<bool>>();
        }

        public Task<Result<UserDTO>> RefreshToken()
        {
            throw new NotImplementedException();
        }

        public async Task<Result<bool>> Register(RegisterDTO user)
        {
            var result = await _http.PostAsJsonAsync("api/account/register", user);
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
