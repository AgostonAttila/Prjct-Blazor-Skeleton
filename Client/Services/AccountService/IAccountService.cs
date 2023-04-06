
using Client.Models;
using Client.Models.DTOs;
using System.Security.Principal;

namespace Client.Services.AccountService
{
    public interface IAccountService
    {     
        Task<Result<UserDTO>> Login(LoginDTO user);
        Task<Result<UserDTO>> FacebookLogin();
        Task<Result<string>> Register(RegisterDTO user);
        Task<Result<string>> VerifyEmail(string token,string email);
        Task<Result<string>> ResendEmailConfirmationLink( string email);
        Task<IIdentity> GetCurrentUser();
        Task<string> RefreshToken();
		Task<bool> Logout();

	}
}
