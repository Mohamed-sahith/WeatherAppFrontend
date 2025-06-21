using Blazored.SessionStorage;
using System.Net.Http.Json;
using System.Security.Claims;
using WeatherAppFrontend.Models.DTOs;

namespace WeatherAppFrontend.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ISessionStorageService _session;

        public AuthService(IHttpClientFactory clientFactory, ISessionStorageService session)
        {
            _httpClient = clientFactory.CreateClient("API");
            _session = session;
        }

        public async Task<bool> Register(RegisterRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Login(LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            if (!response.IsSuccessStatusCode) return false;

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result != null)
            {
                await _session.SetItemAsync("authToken", result.Token);
                await _session.SetItemAsync("email", result.Email);
            }
            return true;
        }

        public async Task Logout()
        {
            await _session.RemoveItemAsync("authToken");
            await _session.RemoveItemAsync("email");
        }

        public async Task<string?> GetToken() => await _session.GetItemAsync<string>("authToken");
    }
}
