using System.Net.Http.Json;
using System.Security.Claims;
using WeatherAppFrontend.Models.DTOs;
using Blazored.LocalStorage;

namespace WeatherAppFrontend.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public AuthService(IHttpClientFactory clientFactory, ILocalStorageService localStorage)
        {
            _httpClient = clientFactory.CreateClient("API");
            _localStorage = localStorage;
        }

        public async Task<bool> Register(RegisterRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Login(LoginRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Login failed: {response.StatusCode}");
                    return false;
                }

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result == null || string.IsNullOrWhiteSpace(result.Token))
                {
                    Console.WriteLine("Login response is null or token is empty.");
                    return false;
                }

                await _localStorage.SetItemAsync("authToken", result.Token);
                Console.WriteLine($"Token stored: {result.Token.Substring(0, Math.Min(10, result.Token.Length))}..."); // Log partial token
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return false;
            }
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            Console.WriteLine("Token removed on logout.");
        }

        public async Task<string?> GetToken()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine("No token found in localStorage.");
            }
            return token;
        }
    }
}