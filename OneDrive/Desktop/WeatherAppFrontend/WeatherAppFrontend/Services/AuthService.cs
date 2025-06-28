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
            try
            {
                Console.WriteLine($"Sending Register request to /api/Auth/register with Email: {request.Email}");
                var response = await _httpClient.PostAsJsonAsync("/api/Auth/register", request);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Register response: Status={response.StatusCode}, Content={responseContent}");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Register failed with status: {response.StatusCode} - {responseContent}");
                    return false;
                }
                return true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Register HTTP error: {ex.Message} - Status={ex.StatusCode}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Register error: {ex.Message}");
                return false;
            }
        }

        public async Task<LoginResponse> Login(LoginRequest request)
        {
            try
            {
                Console.WriteLine($"Sending Login request to /api/Auth/login with Email: {request.Email}");
                var response = await _httpClient.PostAsJsonAsync("/api/Auth/login", request);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Login response: Status={response.StatusCode}, Content={responseContent}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                if (result == null || string.IsNullOrWhiteSpace(result.Token))
                {
                    Console.WriteLine($"Login failed: Invalid or null response. Raw content: {responseContent}");
                    return new LoginResponse();
                }
                await _localStorage.SetItemAsync("authToken", result.Token);
                await _localStorage.SetItemAsync("email", result.Email);
                Console.WriteLine($"Token stored: {result.Token.Substring(0, Math.Min(10, result.Token.Length))}... Email: {result.Email}");
                return result;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Login HTTP error: {ex.Message} - Status={ex.StatusCode}");
                return new LoginResponse();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return new LoginResponse();
            }
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("email");
            Console.WriteLine("Token and email removed on logout.");
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