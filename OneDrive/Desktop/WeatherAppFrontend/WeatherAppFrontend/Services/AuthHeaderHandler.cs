using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace WeatherAppFrontend.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public AuthHeaderHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Console.WriteLine($"Added Authorization header: Bearer {token.Substring(0, Math.Min(10, token.Length))}...");
            }
            else
            {
                Console.WriteLine("No token found in localStorage for request: " + request.RequestUri);
            }

            var response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Console.WriteLine($"Received 401 Unauthorized for {request.RequestUri}. Token may be invalid or expired.");
                // Optionally clear token to force re-login
                // await _localStorage.RemoveItemAsync("authToken");
            }

            return response;
        }
    }
}