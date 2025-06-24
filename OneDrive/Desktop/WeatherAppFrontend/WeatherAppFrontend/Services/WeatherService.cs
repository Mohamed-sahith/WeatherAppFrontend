using System.Net.Http;
using System.Net.Http.Json;
using WeatherAppFrontend.Models.DTOs;

namespace WeatherAppFrontend.Services;

public class WeatherService
{
    private readonly HttpClient _http;

    public WeatherService(IHttpClientFactory factory)
    {
        _http = factory.CreateClient("API") ?? throw new InvalidOperationException("Failed to create HTTP client for API.");
    }

    // Get weather forecast
    public async Task<List<WeatherForecast>> GetForecast(string city)
    {
        try
        {
            var response = await _http.GetFromJsonAsync<List<WeatherForecast>>($"api/weather/forecast/{Uri.EscapeDataString(city)}");
            return response ?? new List<WeatherForecast>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching forecast for {city}: {ex.Message}");
            return new List<WeatherForecast>();
        }
    }

    // Add city to user's favorites
    public async Task AddCityToFavorites(string city)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("api/UserData/add-city", city);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error adding {city} to favorites: {ex.Message}");
            throw;
        }
    }

    // Remove city from user's favorites
    public async Task RemoveCityFromFavorites(string city)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "api/UserData/remove-city")
            {
                Content = JsonContent.Create(city)
            };

            var response = await _http.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error removing {city} from favorites: {ex.Message}");
            throw;
        }
    }

    // Get user's favorite cities
    public async Task<List<string>> GetUserFavorites(string email)
    {
        try
        {
            var response = await _http.GetAsync("api/UserData/cities");
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error fetching favorites for {email}: Status {response.StatusCode}, Content: {errorContent}");
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Optionally trigger re-login or notify user
                    Console.WriteLine("Unauthorized access. Token may be missing or invalid.");
                }
                return new List<string>();
            }

            var cities = await response.Content.ReadFromJsonAsync<List<string>>();
            return cities ?? new List<string>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Error fetching favorites for {email}: {ex.Message}");
            return new List<string>();
        }
    }
}