using Client.Services.AccountService;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Services
{
	public class RefreshTokenService
	{
		private readonly AuthenticationStateProvider _authProvider;
		private readonly IAccountService _accountService;

		public RefreshTokenService(AuthenticationStateProvider authProvider, IAccountService accountService)
		{
			_authProvider = authProvider;
			_accountService = accountService;
		}

		public async Task<string> TryRefreshToken()
		{
			var authState = await _authProvider.GetAuthenticationStateAsync();
			var user = authState.User;

			var exp = user.FindFirst(c => c.Type.Equals("exp")).Value;
			var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));

			var timeUTC = DateTime.UtcNow;

			var diff = expTime - timeUTC;
			if (diff.TotalMinutes <= 2)
				return await _accountService.RefreshToken();

			return string.Empty;
		}
	}
}
