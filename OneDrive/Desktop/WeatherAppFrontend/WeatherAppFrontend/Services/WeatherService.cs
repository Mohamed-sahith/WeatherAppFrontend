using System.Net.Http;
using System.Net.Http.Json;
using WeatherAppFrontend.Models.DTOs;
using WeatherAppFrontend.Services;

namespace WeatherAppFrontend.Services
{
    public class WeatherService
    {
        private readonly HttpClient _http;
        private readonly AuthService _authService;

        public WeatherService(IHttpClientFactory factory, AuthService authService)
        {
            _http = factory.CreateClient("API") ?? throw new InvalidOperationException("Failed to create HTTP client for API.");
            _http.BaseAddress = new Uri("https://localhost:7257/"); // Ensure base address is set
            _authService = authService;
        }

        public async Task<List<WeatherForecast>> GetForecast(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                Console.WriteLine("GetForecast: City name is null or empty.");
                return new List<WeatherForecast>();
            }

            try
            {
                var response = await _http.GetFromJsonAsync<List<WeatherForecast>>($"api/weather/forecast/{Uri.EscapeDataString(city.Trim())}");
                return response ?? new List<WeatherForecast>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"GetForecast: Error fetching forecast for {city}: {ex.Message}");
                return new List<WeatherForecast>();
            }
        }

        public async Task<CityImageResponse?> GetCityImage(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                Console.WriteLine("GetCityImage: City name is null or empty.");
                return null;
            }

            try
            {
                var response = await _http.GetAsync($"api/weather/city-image/{Uri.EscapeDataString(city.Trim())}");
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"GetCityImage: Failed to fetch image for {city}. Status: {response.StatusCode}");
                    return null;
                }

                var cityImage = await response.Content.ReadFromJsonAsync<CityImageResponse>();
                return cityImage;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"GetCityImage: Error fetching city image for {city}: {ex.Message}");
                return null;
            }
        }

        public async Task AddCityToFavorites(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                Console.WriteLine("AddCityToFavorites: City name is null or empty.");
                throw new ArgumentException("City name cannot be null or empty.", nameof(city));
            }

            try
            {
                var token = await _authService.GetToken();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                var response = await _http.PostAsJsonAsync("api/userdata/add-city", city.Trim());
                response.EnsureSuccessStatusCode();
                Console.WriteLine($"AddCityToFavorites: Successfully added {city} to favorites.");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"AddCityToFavorites: Error adding {city} to favorites: {ex.Message}");
                throw;
            }
        }

        public async Task RemoveCityFromFavorites(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                Console.WriteLine("RemoveCityToFavorites: City name is null or empty.");
                throw new ArgumentException("City name cannot be null or empty.", nameof(city));
            }

            try
            {
                var token = await _authService.GetToken();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                var request = new HttpRequestMessage(HttpMethod.Delete, "api/userdata/remove-city")
                {
                    Content = JsonContent.Create(city.Trim())
                };
                var response = await _http.SendAsync(request);
                response.EnsureSuccessStatusCode();
                Console.WriteLine($"RemoveCityFromFavorites: Successfully removed {city} from favorites.");
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"RemoveCityFromFavorites: Error removing {city} from favorites: {ex.Message}");
                throw;
            }
        }

        public async Task<List<string>> GetUserFavorites(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("GetUserFavorites: Email is null or empty.");
                return new List<string>();
            }

            try
            {
                var token = await _authService.GetToken();
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                var requestUri = $"api/userdata/cities?email={Uri.EscapeDataString(email)}";
                Console.WriteLine($"GetUserFavorites: Sending request to {requestUri}");
                var response = await _http.GetAsync(requestUri);
                Console.WriteLine($"GetUserFavorites: Received response with status {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"GetUserFavorites: Failed response content: {content}");
                    return new List<string>();
                }

                var contentString = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"GetUserFavorites: Raw response content: {contentString}");
                var cities = await response.Content.ReadFromJsonAsync<List<string>>();
                Console.WriteLine($"GetUserFavorites: Deserialized {cities?.Count ?? 0} cities: {string.Join(", ", cities ?? new List<string>())}");
                return cities ?? new List<string>();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"GetUserFavorites: Error fetching favorites for {email}: {ex.Message} - Status: {ex.StatusCode}");
                return new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetUserFavorites: Unexpected error for {email}: {ex.Message}");
                return new List<string>();
            }
        }

        // Flagged for hiding - Old/Unused functions
        // [Obsolete("This method is not currently used in UserProfile and is flagged for removal.")]
        // public async Task<bool> AddSearchHistory(string city)
        // {
        //     if (string.IsNullOrWhiteSpace(city))
        //     {
        //         Console.WriteLine("AddSearchHistory: City name is null or empty.");
        //         return false;
        //     }

        //     try
        //     {
        //         var response = await _http.PostAsJsonAsync("api/userdata/add-search", city.Trim());
        //         response.EnsureSuccessStatusCode();
        //         Console.WriteLine($"AddSearchHistory: Successfully added {city} to search history.");
        //         return true;
        //     }
        //     catch (HttpRequestException ex)
        //     {
        //         Console.WriteLine($"AddSearchHistory: Error adding {city} to search history: {ex.Message} - {ex.StatusCode}");
        //         return false;
        //     }
        // }

        // [Obsolete("This method is not currently used in UserProfile and is flagged for removal.")]
        // public async Task<List<string>> GetSearchHistory(string email)
        // {
        //     if (string.IsNullOrWhiteSpace(email))
        //     {
        //         Console.WriteLine("GetSearchHistory: Email is null or empty.");
        //         return new List<string>();
        //     }

        //     try
        //     {
        //         var response = await _http.GetAsync("api/userdata/search-history");
        //         if (!response.IsSuccessStatusCode)
        //         {
        //             Console.WriteLine($"GetSearchHistory: Failed to fetch search history for {email}. Status: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
        //             return new List<string>();
        //         }

        //         var searches = await response.Content.ReadFromJsonAsync<List<string>>();
        //         return searches ?? new List<string>();
        //     }
        //     catch (HttpRequestException ex)
        //     {
        //         Console.WriteLine($"GetSearchHistory: Error fetching search history for {email}: {ex.Message}");
        //         return new List<string>();
        //     }
        // }
    }
}