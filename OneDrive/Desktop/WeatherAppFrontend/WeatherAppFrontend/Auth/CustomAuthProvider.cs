using System.Security.Claims;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace WeatherAppFrontend.Auth
{
    public class CustomAuthProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _session;

        public CustomAuthProvider(ISessionStorageService session)
        {
            _session = session;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _session.GetItemAsync<string>("authToken");
            var email = await _session.GetItemAsync<string>("email");

            ClaimsIdentity identity = string.IsNullOrWhiteSpace(token) ? new ClaimsIdentity() :
                new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email!) }, "apiauth");

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public void NotifyAuthChanged() => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
