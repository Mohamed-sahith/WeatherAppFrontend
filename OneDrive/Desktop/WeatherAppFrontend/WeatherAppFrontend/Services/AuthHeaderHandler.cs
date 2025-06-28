using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace WeatherAppFrontend.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authProvider;

        public AuthHeaderHandler(ILocalStorageService localStorage, AuthenticationStateProvider authProvider)
        {
            _localStorage = localStorage;
            _authProvider = authProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authState = await _authProvider.GetAuthenticationStateAsync();
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token) && !request.RequestUri!.AbsolutePath.Contains("login"))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                Console.WriteLine($"Added Authorization header with token for {request.RequestUri}");
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}