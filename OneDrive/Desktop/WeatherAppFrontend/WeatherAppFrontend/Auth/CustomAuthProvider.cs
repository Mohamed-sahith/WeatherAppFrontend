using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace WeatherAppFrontend.Auth
{
    public class CustomAuthProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            ClaimsIdentity identity;

            if (!string.IsNullOrWhiteSpace(token))
            {
                var email = GetEmailFromToken(token);

                if (!string.IsNullOrWhiteSpace(email))
                {
                    identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, email)
                    }, "apiauth");
                }
                else
                {
                    identity = new ClaimsIdentity();
                }
            }
            else
            {
                identity = new ClaimsIdentity();
            }

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public void NotifyAuthChanged()
        {
            var task = GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(task);
        }

        private string? GetEmailFromToken(string jwt)
        {
            try
            {
                var parts = jwt.Split('.');
                if (parts.Length != 3)
                    return null;

                var payload = parts[1];
                // Fix base64 padding
                payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');

                var jsonBytes = Convert.FromBase64String(payload);
                var json = Encoding.UTF8.GetString(jsonBytes);

                var payloadData = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                return payloadData != null && payloadData.TryGetValue("email", out var emailObj)
                    ? emailObj?.ToString()
                    : null;
            }
            catch
            {
                return null;
            }
        }
    }
}
