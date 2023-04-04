
using Client.Models;
using Client.Models.DTOs;


namespace Client.Services.AccountService
{
    public interface IAccountService
    {     
        Task<Result<bool>> Login(LoginDTO user);
        Task<Result<bool>> FacebookLogin();
        Task<Result<bool>> Register(RegisterDTO user);
        Task<Result<string>> VerifyEmail(string token,string email);
        Task<Result<string>> ResendEmailConfirmationLink( string email);
        Task<Result<UserDTO>> GetCurrentUser();
        Task<Result<UserDTO>> RefreshToken();

    }
}
