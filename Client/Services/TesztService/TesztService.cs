using Blazored.LocalStorage;
using Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace Client.Services.TesztService
{
	public class TesztService : ITesztService
	{
		private readonly HttpClient _httpClient;
		
		public TesztService(HttpClient httpClient)
		{
			_httpClient = httpClient;				
		}

		public async Task<Result<string>> Teszt1()
		{
			
			var result = await _httpClient.GetAsync("teszt/teszt1");
			var content = await result.Content.ReadAsStringAsync();
			var resultJSON = JsonSerializer.Deserialize<Result<string>>(content);
			return new Result<string> { IsSuccess = true, Data = resultJSON?.Data }; 
		}

		public async Task<Result<string>> Teszt2()
		{

			var result = await _httpClient.GetAsync("teszt/teszt2");
			var content = await result.Content.ReadAsStringAsync();
			var resultJSON = JsonSerializer.Deserialize<Result<string>>(content);
			return new Result<string> { IsSuccess = true, Data = resultJSON?.Data };
		}
	}
}
