using Client.Models;

namespace Client.Services.TesztService
{
	public interface ITesztService
	{
		Task<Result<string>> Teszt1();

		Task<Result<string>> Teszt2();
	}
}
